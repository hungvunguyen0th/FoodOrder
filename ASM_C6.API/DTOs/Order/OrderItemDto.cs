using System.ComponentModel.DataAnnotations;

namespace ASM_C6.API.DTOs.Order
{
    public class OrderItemDto
    {
        public int? FoodItemId { get; set; }
        public int? ComboId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? Notes { get; set; }
        public List<int> ToppingIds { get; set; } = new();
    }
}