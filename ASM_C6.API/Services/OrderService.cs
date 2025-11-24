using ASM_C6.API.Data;
using ASM_C6.API.DTOs.Order;
using ASM_C6.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ASM_C6.API.Services
{
    public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;
    public OrderService(ApplicationDbContext context) { _context = context; }

    public async Task<(bool Success, string Message, OrderDto? Order)> CreateOrderAsync(CreateOrderDto dto, string? currentUserId = null)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Check stock
            foreach (var item in dto.Items)
            {
                if (item.FoodItemId.HasValue)
                {
                    var f = await _context.FoodItems.FindAsync(item.FoodItemId.Value);
                    if (f == null || f.Stock < item.Quantity)
                        return (false, $"Món {item.ItemName} không đủ số lượng", null);
                }
            }
            var order = new Order
            {
                OrderNumber = GenerateOrderNumber(),
                UserId = dto.UserId,
                CustomerName = dto.CustomerName,
                PhoneNumber = dto.PhoneNumber,
                DeliveryAddress = dto.DeliveryAddress,
                Ward = dto.Ward,
                District = dto.District,
                PaymentMethod = (PaymentMethod)dto.PaymentMethod,
                OrderType = (OrderType)dto.OrderType,
                Status = OrderStatus.Pending,
                Notes = dto.Notes,
                CreatedByStaffId = currentUserId,
                CreatedDate = DateTime.Now,
                IsPaid = false
            };
            decimal subtotal = 0;
            foreach (var itemDto in dto.Items)
            {
                var orderItem = new OrderItem
                {
                    FoodItemId = itemDto.FoodItemId,
                    ComboId = itemDto.ComboId,
                    ItemName = itemDto.ItemName,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice,
                    Notes = itemDto.Notes
                };
                if (itemDto.ToppingIds != null && itemDto.ToppingIds.Any())
                {
                    foreach (var tid in itemDto.ToppingIds)
                    {
                        var topping = await _context.Toppings.FindAsync(tid);
                        if (topping != null && topping.IsAvailable)
                        {
                            orderItem.Toppings.Add(new OrderItemTopping
                            {
                                ToppingId = tid,
                                ToppingName = topping.Name,
                                Price = topping.Price
                            });
                            orderItem.UnitPrice += topping.Price;
                        }
                    }
                }
                orderItem.TotalPrice = orderItem.UnitPrice * orderItem.Quantity;
                subtotal += orderItem.TotalPrice;
                order.OrderItems.Add(orderItem);

                // Giảm tồn kho food
                if (itemDto.FoodItemId.HasValue)
                {
                    var food = await _context.FoodItems.FindAsync(itemDto.FoodItemId.Value);
                    if (food != null)
                    {
                        food.Stock -= itemDto.Quantity;
                        food.PurchaseCount += itemDto.Quantity;
                    }
                }
            }
            order.SubTotal = subtotal;
            order.TotalAmount = subtotal;
            if (dto.OrderType == (int)OrderType.Delivery)
            {
                order.ShippingFee = CalculateShippingFee(subtotal);
                order.TotalAmount += order.ShippingFee;
            }
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            var orderDto = await GetOrderByIdAsync(order.Id);
            return (true, "Đặt hàng thành công", orderDto);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return (false, $"Lỗi khi tạo đơn hàng: {ex.Message}", null);
        }
    }

    public async Task<List<OrderDto>> GetOrdersAsync(string? userId = null, int? status = null, int pageNumber = 1, int pageSize = 10)
    {
        var q = _context.Orders
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Toppings)
            .Include(o => o.User).AsQueryable();
        if (!string.IsNullOrEmpty(userId)) q = q.Where(x => x.UserId == userId);
        if (status.HasValue) q = q.Where(x => (int)x.Status == status.Value);

        var orderList = await q.OrderByDescending(o => o.CreatedDate)
            .Skip((pageNumber - 1) * pageSize).Take(pageSize)
            .ToListAsync();

        return orderList.Select(MapToDto).ToList();
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int id)
    {
        var o = await _context.Orders
            .Include(xx => xx.OrderItems).ThenInclude(x => x.Toppings)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id);
        return o == null ? null : MapToDto(o);
    }

    public async Task<OrderDto?> GetOrderByNumberAsync(string orderNumber)
    {
        var o = await _context.Orders
            .Include(xx => xx.OrderItems).ThenInclude(x => x.Toppings)
            .FirstOrDefaultAsync(x => x.OrderNumber == orderNumber);
        return o == null ? null : MapToDto(o);
    }

    public async Task<bool> UpdateOrderStatusAsync(int id, int status)
    {
        var o = await _context.Orders.FindAsync(id);
        if (o == null) return false;
        o.Status = (OrderStatus)status;
        o.UpdatedDate = DateTime.Now;
        if (status == (int)OrderStatus.Completed)
        {
            o.IsPaid = true;
            o.CompletedDate = DateTime.Now;
        }
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelOrderAsync(int id, string? userId = null)
    {
        var o = await _context.Orders.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.Id == id);
        if (o == null) return false;
        if (o.Status != OrderStatus.Pending && o.Status != OrderStatus.Confirmed) return false;
        if (!string.IsNullOrEmpty(userId) && o.UserId != userId) return false;
        o.Status = OrderStatus.Cancelled;
        o.UpdatedDate = DateTime.Now;

        // Trả lại kho
        foreach (var item in o.OrderItems)
        {
            if (item.FoodItemId.HasValue)
            {
                var food = await _context.FoodItems.FindAsync(item.FoodItemId.Value);
                if (food != null)
                {
                    food.Stock += item.Quantity;
                    food.PurchaseCount -= item.Quantity;
                }
            }
        }
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Dictionary<string, int>> GetOrderStatisticsAsync()
    {
        var d = new Dictionary<string, int>();
        d["Total"] = await _context.Orders.CountAsync();
        d["Pending"] = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Pending);
        d["Completed"] = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Completed);
        d["Cancelled"] = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Cancelled);
        return d;
    }

    // Helpers
    private static string GenerateOrderNumber() => $"ORD{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000,9999)}";
    private static decimal CalculateShippingFee(decimal subTotal)
        => subTotal >= 100000 ? 0 : (subTotal >= 50000 ? 15000 : 25000);

    private static OrderDto MapToDto(Order o) => new OrderDto
    {
        Id = o.Id,
        OrderNumber = o.OrderNumber,
        CustomerName = o.CustomerName,
        PhoneNumber = o.PhoneNumber,
        DeliveryAddress = o.DeliveryAddress,
        Ward = o.Ward,
        District = o.District,
        SubTotal = o.SubTotal,
        DiscountAmount = o.DiscountAmount,
        ShippingFee = o.ShippingFee,
        TotalAmount = o.TotalAmount,
        Status = (int)o.Status,
        StatusText = o.Status.ToString(),
        PaymentMethod = (int)o.PaymentMethod,
        PaymentMethodText = o.PaymentMethod.ToString(),
        IsPaid = o.IsPaid,
        OrderType = (int)o.OrderType,
        OrderTypeText = o.OrderType.ToString(),
        CreatedDate = o.CreatedDate,
        UpdatedDate = o.UpdatedDate,
        CompletedDate = o.CompletedDate,
        Notes = o.Notes,
        UserId = o.UserId,
        UserName = o.User?.FullName,
        CreatedByStaffId = o.CreatedByStaffId,
        Items = o.OrderItems.Select(oi => new OrderItemDetailDto
        {
            Id = oi.Id,
            ItemName = oi.ItemName,
            Quantity = oi.Quantity,
            UnitPrice = oi.UnitPrice,
            TotalPrice = oi.TotalPrice,
            Notes = oi.Notes,
            Toppings = oi.Toppings.Select(t => new ToppingDetailDto
            {
                Id = t.Id,
                Name = t.ToppingName,
                Price = t.Price
            }).ToList()
        }).ToList()
    };
}

}
