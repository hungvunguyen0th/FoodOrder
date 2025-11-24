using System.ComponentModel.DataAnnotations;

namespace ASM_C6.API.DTOs.Food
{
    public class IngredientDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public string? Unit { get; set; }
        public decimal Quantity { get; set; }
    }
}