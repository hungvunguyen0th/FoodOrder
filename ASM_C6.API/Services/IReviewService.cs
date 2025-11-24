using ASM_C6.API.DTOs.Review;

namespace ASM_C6.API.Services
{
    public interface IReviewService
    {
        Task<List<ReviewDto>> GetFoodReviewsAsync(int foodItemId);
        Task<List<ReviewDto>> GetUserReviewsAsync(string userId);
        Task<(bool Success, string Message, ReviewDto? Review)> CreateReviewAsync(ReviewDto dto, string userId);
        Task<bool> ApproveReviewAsync(int id);
        Task<bool> DeleteReviewAsync(int id);
    }
}