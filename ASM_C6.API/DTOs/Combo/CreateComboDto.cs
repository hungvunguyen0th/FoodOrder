using System.ComponentModel.DataAnnotations;

namespace ASM_C6.API.DTOs.Combo
{
    public class CreateComboDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public decimal? DiscountPercent { get; set; }
        public List<ComboItemDto> Items { get; set; } = new();
    }
}