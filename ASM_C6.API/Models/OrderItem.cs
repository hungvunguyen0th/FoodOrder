namespace ASM_C6.API.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int? FoodItemId { get; set; }
        public int? ComboId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Notes { get; set; }

        public Order Order { get; set; } = null!;
        public FoodItem? FoodItem { get; set; }
        public Combo? Combo { get; set; }
        public ICollection<OrderItemTopping> Toppings { get; set; } = new List<OrderItemTopping>();
    }
}