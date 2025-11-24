using ASM_C6.API.Models;
using System.ComponentModel.DataAnnotations;

namespace ASM_C6.API.DTOs.Order
{
    public class CreateOrderDto
    {
        public string? UserId { get; set; }
        public string? CustomerName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? Ward { get; set; }
        public string? District { get; set; }
        public int PaymentMethod { get; set; }
        public int OrderType { get; set; }
        public string? Notes { get; set; }
        public string? DiscountCode { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}