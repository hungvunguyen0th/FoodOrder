using ASM_C6.API.Models;

namespace ASM_C6.API.DTOs.Order
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string? CustomerName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? Ward { get; set; }
        public string? District { get; set; }
        public decimal SubTotal { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalAmount { get; set; }
        public int Status { get; set; }
        public string StatusText { get; set; } = string.Empty;
        public int PaymentMethod { get; set; }
        public string PaymentMethodText { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
        public int OrderType { get; set; }
        public string OrderTypeText { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string? Notes { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? CreatedByStaffId { get; set; }
        public string? CreatedByStaffName { get; set; }
        public List<OrderItemDetailDto> Items { get; set; } = new();
    }
}