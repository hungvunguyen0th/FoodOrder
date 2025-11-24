namespace ASM_C6.API.Models
{
    public class FoodIngredient
    {
        public int Id { get; set; }
        public int FoodItemId { get; set; }
        public string IngredientName { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public string? Unit { get; set; }
        public decimal Quantity { get; set; }

        // Navigation
        public FoodItem FoodItem { get; set; } = null!;
    }
}