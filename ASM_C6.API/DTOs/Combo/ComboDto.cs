using System.ComponentModel.DataAnnotations;

namespace ASM_C6.API.DTOs.Combo
{
    public class ComboDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public int PurchaseCount { get; set; }
        public decimal? DiscountPercent { get; set; }
        public List<ComboItemDto> Items { get; set; } = new();
        public decimal OriginalPrice { get; set; }
        public decimal SavedAmount { get; set; }
    }
}