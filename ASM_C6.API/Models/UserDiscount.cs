namespace ASM_C6.API.Models
{
    public class UserDiscount
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int DiscountId { get; set; }
        public bool IsUsed { get; set; }
        public DateTime? UsedDate { get; set; }
        public DateTime AssignedDate { get; set; } = DateTime.Now;

        public ApplicationUser User { get; set; } = null!;
        public Discount Discount { get; set; } = null!;
    }
}