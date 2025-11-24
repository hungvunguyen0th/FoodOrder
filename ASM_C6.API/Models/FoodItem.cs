namespace ASM_C6.API.Models
{
    public class FoodItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal BasePrice { get; set; }
        public decimal Cost { get; set; }
        public string? ImageUrl { get; set; }
        public int Calories { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; }
        public int CategoryId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
        public int ViewCount { get; set; }
        public int PurchaseCount { get; set; }

        // Navigation
        public FoodCategory Category { get; set; } = null!;
        public ICollection<FoodIngredient> Ingredients { get; set; } = new List<FoodIngredient>();
        public ICollection<FoodTopping> FoodToppings { get; set; } = new List<FoodTopping>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<ComboItem> ComboItems { get; set; } = new List<ComboItem>();
    }
}