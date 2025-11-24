using ASM_C6.API.DTOs.Order;
using ASM_C6.API.Models;

namespace ASM_C6.API.Services
{
    public interface IOrderService
    {
        Task<(bool Success, string Message, OrderDto? Order)> CreateOrderAsync(CreateOrderDto dto, string? currentUserId = null);
        Task<List<OrderDto>> GetOrdersAsync(string? userId = null, int? status = null, int pageNumber = 1, int pageSize = 10);
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<OrderDto?> GetOrderByNumberAsync(string orderNumber);
        Task<bool> UpdateOrderStatusAsync(int id, int status);
        Task<bool> CancelOrderAsync(int id, string? userId = null);
        Task<Dictionary<string, int>> GetOrderStatisticsAsync();
    }
}