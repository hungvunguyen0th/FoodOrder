using ASM_C6.API.Data;
using ASM_C6.API.DTOs.Discount;
using ASM_C6.API.Models;
using Microsoft.EntityFrameworkCore;


namespace ASM_C6.API.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly ApplicationDbContext _context;

        public DiscountService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DiscountDto>> GetAllDiscountsAsync(bool? isActive = null)
        {
            var q = _context.Discounts.AsQueryable();
            if (isActive.HasValue)
                q = q.Where(d => d.IsActive == isActive.Value);

            // Lấy entity từ database về trước
            var discountList = await q.OrderByDescending(d => d.CreatedDate).ToListAsync();

            // Map sang DTO ở phía memory, không lỗi ToListAsync
            return discountList.Select(MapDiscountToDto).ToList();
        }


        public async Task<DiscountDto?> GetDiscountByIdAsync(int id)
        {
            var d = await _context.Discounts.FindAsync(id);
            return d == null ? null : MapDiscountToDto(d);
        }

        public async Task<DiscountDto?> GetDiscountByCodeAsync(string code)
        {
            var d = await _context.Discounts.FirstOrDefaultAsync(x => x.Code == code);
            return d == null ? null : MapDiscountToDto(d);
        }

        public async Task<DiscountDto> CreateDiscountAsync(DiscountDto dto)
        {
            var d = new Discount
            {
                Code = dto.Code.ToUpper(),
                Description = dto.Description,
                DiscountPercent = dto.DiscountPercent,
                MaxDiscountAmount = dto.MaxDiscountAmount,
                MinOrderAmount = dto.MinOrderAmount,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Type = (DiscountType)dto.Type,
                IsActive = dto.IsActive,
                UsageLimit = dto.UsageLimit,
                CreatedDate = DateTime.Now
            };
            _context.Discounts.Add(d);
            await _context.SaveChangesAsync();
            return MapDiscountToDto(d);
        }

        public async Task<bool> UpdateDiscountAsync(int id, DiscountDto dto)
        {
            var d = await _context.Discounts.FindAsync(id);
            if (d == null) return false;
            d.Code = dto.Code.ToUpper();
            d.Description = dto.Description;
            d.DiscountPercent = dto.DiscountPercent;
            d.MaxDiscountAmount = dto.MaxDiscountAmount;
            d.MinOrderAmount = dto.MinOrderAmount;
            d.StartDate = dto.StartDate;
            d.EndDate = dto.EndDate;
            d.Type = (DiscountType)dto.Type;
            d.IsActive = dto.IsActive;
            d.UsageLimit = dto.UsageLimit;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteDiscountAsync(int id)
        {
            var d = await _context.Discounts.FindAsync(id);
            if (d == null) return false;
            _context.Discounts.Remove(d);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool IsValid, string Message, DiscountDto? Discount)> ValidateDiscountAsync(string code,
            decimal orderAmount, string? userId = null)
        {
            var d = await _context.Discounts.Include(x => x.UserDiscounts).FirstOrDefaultAsync(x => x.Code == code);
            if (d == null) return (false, "Không tồn tại mã giảm giá", null);

            if (!d.IsActive || DateTime.Now < d.StartDate || DateTime.Now > d.EndDate)
                return (false, "Mã giảm giá không còn hiệu lực", null);

            if (d.MinOrderAmount.HasValue && orderAmount < d.MinOrderAmount.Value)
                return (false, $"Đơn hàng phải tối thiểu {d.MinOrderAmount.Value:N0}đ", null);

            if (d.UsageLimit.HasValue && d.UsedCount >= d.UsageLimit)
                return (false, "Mã giảm giá đã hết lượt sử dụng", null);

            if (d.Type == DiscountType.Personal && !string.IsNullOrEmpty(userId))
            {
                var ud = await _context.UserDiscounts.FirstOrDefaultAsync(u =>
                    u.DiscountId == d.Id && u.UserId == userId);
                if (ud == null) return (false, "Mã này không áp dụng cho bạn", null);
                if (ud.IsUsed) return (false, "Bạn đã dùng mã này rồi", null);
            }

            var dto = MapDiscountToDto(d);
            dto.IsValid = true;
            return (true, "Mã hợp lệ", dto);
        }

        public async Task<bool> AssignDiscountToUserAsync(int discountId, string userId)
        {
            var d = await _context.Discounts.FindAsync(discountId);
            if (d == null || d.Type != DiscountType.Personal) return false;

            var exists =
                await _context.UserDiscounts.AnyAsync(ud => ud.DiscountId == discountId && ud.UserId == userId);
            if (exists) return false;

            _context.UserDiscounts.Add(new UserDiscount
            {
                DiscountId = discountId,
                UserId = userId,
                IsUsed = false,
                AssignedDate = DateTime.Now
            });
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<DiscountDto>> GetUserDiscountsAsync(string userId)
        {
            var userD = await _context.UserDiscounts.Include(ud => ud.Discount)
                .Where(ud => ud.UserId == userId && !ud.IsUsed).Select(ud => ud.Discount).ToListAsync();

            var generalD = await _context.Discounts.Where(d =>
                d.Type == DiscountType.General && d.IsActive && d.StartDate <= DateTime.Now &&
                d.EndDate >= DateTime.Now).ToListAsync();

            return userD.Concat(generalD).Distinct().Select(MapDiscountToDto).ToList();
        }

        private static DiscountDto MapDiscountToDto(Discount d)
        {
            var isValid = d.IsActive && d.StartDate <= DateTime.Now && d.EndDate >= DateTime.Now
                          && (!d.UsageLimit.HasValue || d.UsedCount < d.UsageLimit.Value);

            return new DiscountDto
            {
                Id = d.Id,
                Code = d.Code,
                Description = d.Description,
                DiscountPercent = d.DiscountPercent,
                MaxDiscountAmount = d.MaxDiscountAmount,
                MinOrderAmount = d.MinOrderAmount,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                Type = (int)d.Type,
                IsActive = d.IsActive,
                UsageLimit = d.UsageLimit,
                UsedCount = d.UsedCount,
                IsValid = isValid,
                InvalidReason = !isValid ? GetInvalidReason(d) : null
            };
        }

        private static string GetInvalidReason(Discount d)
        {
            if (!d.IsActive) return "Mã đã bị vô hiệu hóa";
            if (DateTime.Now < d.StartDate) return "Chưa đến thời gian sử dụng";
            if (DateTime.Now > d.EndDate) return "Đã hết hạn";
            if (d.UsageLimit.HasValue && d.UsedCount >= d.UsageLimit.Value) return "Đã hết lượt sử dụng";
            return string.Empty;
        }
    }
}
