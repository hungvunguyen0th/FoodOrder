using ASM_C6.API.Data;
using ASM_C6.API.DTOs.Review;
using ASM_C6.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ASM_C6.API.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;
        public ReviewService(ApplicationDbContext context) { _context = context; }

        public async Task<List<ReviewDto>> GetFoodReviewsAsync(int foodItemId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.FoodItem)
                .Where(r => r.FoodItemId == foodItemId && r.IsApproved)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();

            return reviews.Select(MapReviewToDto).ToList();
        }

        public async Task<List<ReviewDto>> GetUserReviewsAsync(string userId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.FoodItem)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();

            return reviews.Select(MapReviewToDto).ToList();
        }

        public async Task<(bool Success, string Message, ReviewDto? Review)> CreateReviewAsync(ReviewDto dto, string userId)
        {
            var exists = await _context.Reviews.AnyAsync(r => r.FoodItemId == dto.FoodItemId && r.UserId == userId);
            if (exists) return (false, "Bạn đã đánh giá món này rồi", null);

            var r = new Review
            {
                FoodItemId = dto.FoodItemId,
                UserId = userId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedDate = DateTime.Now,
                IsApproved = false
            };
            _context.Reviews.Add(r);
            await _context.SaveChangesAsync();

            var detailEntity = await _context.Reviews.Include(rr => rr.User).Include(rr => rr.FoodItem)
                .Where(rr => rr.Id == r.Id).FirstOrDefaultAsync();
            var detail = detailEntity != null ? MapReviewToDto(detailEntity) : null;
            return (true, "Đánh giá đang chờ duyệt", detail);
        }

        public async Task<bool> ApproveReviewAsync(int id)
        {
            var r = await _context.Reviews.FindAsync(id);
            if (r == null) return false;
            r.IsApproved = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteReviewAsync(int id)
        {
            var r = await _context.Reviews.FindAsync(id);
            if (r == null) return false;
            _context.Reviews.Remove(r);
            await _context.SaveChangesAsync();
            return true;
        }

        private static ReviewDto MapReviewToDto(Review r) => new ReviewDto
        {
            Id = r.Id,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedDate = r.CreatedDate,
            FoodItemId = r.FoodItemId,
            FoodItemName = r.FoodItem?.Name,
            UserId = r.UserId,
            UserName = r.User?.FullName,
            UserAvatar = r.User?.Avatar,
            IsApproved = r.IsApproved
        };
    }

}
