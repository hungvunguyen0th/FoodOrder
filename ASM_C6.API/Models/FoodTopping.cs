namespace ASM_C6.API.Models
{
    public class FoodTopping
    {
        public int FoodItemId { get; set; }
        public int ToppingId { get; set; }

        public FoodItem FoodItem { get; set; } = null!;
        public Topping Topping { get; set; } = null!;
    }
}