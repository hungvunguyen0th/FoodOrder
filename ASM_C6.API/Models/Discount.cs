namespace ASM_C6.API.Models
{
    public enum DiscountType { General = 0, Personal = 1 }

    public class Discount
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DiscountType Type { get; set; }
        public bool IsActive { get; set; }
        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public ICollection<UserDiscount> UserDiscounts { get; set; } = new List<UserDiscount>();
    }
}