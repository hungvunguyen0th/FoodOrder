using System.ComponentModel.DataAnnotations;

namespace ASM_C6.API.DTOs.Review
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedDate { get; set; }
        public int FoodItemId { get; set; }
        public string? FoodItemName { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? UserAvatar { get; set; }
        public bool IsApproved { get; set; }
    }
}