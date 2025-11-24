using System.ComponentModel.DataAnnotations;

namespace ASM_C6.API.DTOs.Topping
{
    public class ToppingDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
    }
}