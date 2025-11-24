namespace ASM_C6.API.Models
{
    public class ComboItem
    {
        public int Id { get; set; }
        public int ComboId { get; set; }
        public int FoodItemId { get; set; }
        public int Quantity { get; set; }

        public Combo Combo { get; set; } = null!;
        public FoodItem FoodItem { get; set; } = null!;
    }
}