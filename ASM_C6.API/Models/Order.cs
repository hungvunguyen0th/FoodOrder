namespace ASM_C6.API.Models
{
    public enum OrderStatus { Pending, Confirmed, Preparing, ReadyForDelivery, Delivering, Completed, Cancelled }
    public enum PaymentMethod { Cash, BankTransfer, QRCode, MoMo, VNPay }
    public enum OrderType { Delivery, Onsite }

    public class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public string? CustomerName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? Ward { get; set; }
        public string? District { get; set; }
        public decimal SubTotal { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsPaid { get; set; }
        public OrderType OrderType { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string? Notes { get; set; }
        public int? DiscountId { get; set; }
        public string? CreatedByStaffId { get; set; }

        // Navigation
        public ApplicationUser? User { get; set; }
        public ApplicationUser? CreatedByStaff { get; set; }
        public Discount? Discount { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
