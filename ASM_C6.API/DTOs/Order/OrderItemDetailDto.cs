namespace ASM_C6.API.DTOs.Order
{
    public class OrderItemDetailDto
    {
        public int Id { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Notes { get; set; }
        public List<ToppingDetailDto> Toppings { get; set; } = new();
    }

    public class ToppingDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}