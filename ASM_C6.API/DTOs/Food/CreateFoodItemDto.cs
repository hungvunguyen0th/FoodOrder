using System.ComponentModel.DataAnnotations;

namespace ASM_C6.API.DTOs.Food
{
    public class CreateFoodItemDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int Calories { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; }
        public List<IngredientDto> Ingredients { get; set; } = new();
        public List<int> ToppingIds { get; set; } = new();
    }
}