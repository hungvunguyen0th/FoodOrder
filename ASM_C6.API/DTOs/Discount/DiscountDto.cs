using ASM_C6.API.Models;
using System.ComponentModel.DataAnnotations;

namespace ASM_C6.API.DTOs.Discount
{
    public class DiscountDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Type { get; set; }
        public bool IsActive { get; set; }
        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public bool IsValid { get; set; }
        public string? InvalidReason { get; set; }
    }
}