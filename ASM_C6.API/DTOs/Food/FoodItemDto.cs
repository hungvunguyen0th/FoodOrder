using System.ComponentModel.DataAnnotations;

namespace ASM_C6.API.DTOs.Food
{
    public class FoodItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public string? ImageUrl { get; set; }
        public int Calories { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public List<IngredientDto> Ingredients { get; set; } = new();
        public List<int> ToppingIds { get; set; } = new();
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }

        public bool IsAvailable => IsActive && Stock > 0;
        public int ViewCount { get; set; }
        public int PurchaseCount { get; set; }
    }
}