using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Data;
using OrderManagementSystem.DTOs;
using OrderManagementSystem.Models;
using System.Security.Claims;

namespace OrderManagementSystem.Services;

public class OrderService
{
    private readonly AppDbContext _db;
    private readonly IHttpContextAccessor _http;
    public OrderService(AppDbContext db, IHttpContextAccessor http)
    {
        _db = db;
        _http = http;
    }

    private Guid GetUserId()
    {
        var id = _http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(id, out var guid) ? guid : throw new Exception("User not authenticated");
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderRequest req)
    {
        if (req.Items.Count == 0)
            throw new InvalidOperationException("Order must contain at least one item.");
        var ids = req.Items.Select(i => i.ProductId).ToList();
        var products = await _db.Products.Where(p => ids.Contains(p.Id)).ToDictionaryAsync(p => p.Id);
        foreach (var item in req.Items)
        {
            if (!products.TryGetValue(item.ProductId, out var p))
                throw new KeyNotFoundException($"Product {item.ProductId} not found.");
            if (item.Quantity <= 0)
                throw new InvalidOperationException("Quantity must be greater than zero.");
            if (p.Stock < item.Quantity)
                throw new InvalidOperationException($"Insufficient stock for product {p.Name}.");
        }
        using var tx = await _db.Database.BeginTransactionAsync();
        var order = new Order
        {
            UserId = GetUserId(),
            CreatedAt = DateTime.UtcNow
        };
        foreach (var item in req.Items)
        {
            var p = products[item.ProductId];
            p.Stock -= item.Quantity;
            order.Items.Add(new OrderItem
            {
                ProductId = p.Id,
                Quantity = item.Quantity,
                Price = p.Price
            });
        }
        order.TotalPrice = order.Items.Sum(i => i.Price * i.Quantity);
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        await tx.CommitAsync();
        return await MapOrderDto(order.Id);
    }

    public async Task<List<OrderDto>> GetMyOrdersAsync()
    {
        var userId = GetUserId();
        return await _db.Orders.AsNoTracking()
            .Where(o => o.UserId == userId)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderDto
            {
                OrderId = o.Id,
                CreatedAt = o.CreatedAt,
                TotalPrice = o.TotalPrice,
                Items = o.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product != null ? i.Product.Name : string.Empty,
                    Quantity = i.Quantity,
                    Price = i.Price,
                    LineTotal = i.Price * i.Quantity
                }).ToList()
            })
            .ToListAsync();
    }

    private IQueryable<OrderDto> QueryOrders()
        => _db.Orders.AsNoTracking()
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Select(o => new OrderDto
            {
                OrderId = o.Id,
                CreatedAt = o.CreatedAt,
                TotalPrice = o.TotalPrice,
                Items = o.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product != null ? i.Product.Name : string.Empty,
                    Quantity = i.Quantity,
                    Price = i.Price,
                    LineTotal = i.Price * i.Quantity
                }).ToList()
            });

    private async Task<OrderDto> MapOrderDto(Guid id)
        => await QueryOrders().FirstAsync(o => o.OrderId == id);
}
