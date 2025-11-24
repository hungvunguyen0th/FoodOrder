namespace ASM_C6.API.Models
{
    public class OrderItemTopping
    {
        public int Id { get; set; }
        public int OrderItemId { get; set; }
        public int ToppingId { get; set; }
        public string ToppingName { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public OrderItem OrderItem { get; set; } = null!;
        public Topping Topping { get; set; } = null!;
    }
}