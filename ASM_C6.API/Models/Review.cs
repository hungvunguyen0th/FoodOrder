namespace ASM_C6.API.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int Rating { get; set; } // 1-5
        public string? Comment { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int FoodItemId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public bool IsApproved { get; set; } = false;

        public FoodItem FoodItem { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }
}