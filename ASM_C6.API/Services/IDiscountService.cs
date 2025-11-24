using ASM_C6.API.DTOs.Discount;

namespace ASM_C6.API.Services
{
    public interface IDiscountService
    {
        Task<List<DiscountDto>> GetAllDiscountsAsync(bool? isActive = null);
        Task<DiscountDto?> GetDiscountByIdAsync(int id);
        Task<DiscountDto?> GetDiscountByCodeAsync(string code);
        Task<DiscountDto> CreateDiscountAsync(DiscountDto dto);
        Task<bool> UpdateDiscountAsync(int id, DiscountDto dto);
        Task<bool> DeleteDiscountAsync(int id);
        Task<(bool IsValid, string Message, DiscountDto? Discount)> ValidateDiscountAsync(string code, decimal orderAmount, string? userId = null);
        Task<bool> AssignDiscountToUserAsync(int discountId, string userId);
        Task<List<DiscountDto>> GetUserDiscountsAsync(string userId);
    }
}