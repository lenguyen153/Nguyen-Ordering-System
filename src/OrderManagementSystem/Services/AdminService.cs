using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Data;
using OrderManagementSystem.DTOs;

namespace OrderManagementSystem.Services;

public class AdminService
{
    private readonly AppDbContext _db;
    public AdminService(AppDbContext db) => _db = db;

    public Task<List<OrderDto>> GetAllOrdersAsync()
        => _db.Orders.AsNoTracking()
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

    public async Task<List<SalesStatPoint>> GetSalesStatsAsync(SalesRange range)
    {
        var orders = _db.Orders.AsNoTracking();
        IQueryable<SalesStatPoint> q = range switch
        {
            SalesRange.Day => orders
                .GroupBy(o => new DateTime(o.CreatedAt.Year, o.CreatedAt.Month, o.CreatedAt.Day))
                .Select(g => new SalesStatPoint { PeriodStart = g.Key, TotalSales = g.Sum(o => o.TotalPrice) }),
            SalesRange.Week => orders
                .GroupBy(o =>
                    o.CreatedAt.AddDays(-(int)o.CreatedAt.DayOfWeek)
                )
                .Select(g => new SalesStatPoint { PeriodStart = g.Key, TotalSales = g.Sum(o => o.TotalPrice) }),
            SalesRange.Month => orders
                .GroupBy(o => new DateTime(o.CreatedAt.Year, o.CreatedAt.Month, 1))
                .Select(g => new SalesStatPoint { PeriodStart = g.Key, TotalSales = g.Sum(o => o.TotalPrice) }),
            _ => throw new ArgumentOutOfRangeException(nameof(range))
        };
        return await q.OrderBy(x => x.PeriodStart).ToListAsync();
    }

    public async Task<List<SalesStatPoint>> GetStatsByDayAsync(DateTime date)
    {
        var dayStart = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
        var dayEnd = dayStart.AddDays(1);
        var total = await _db.Orders.AsNoTracking()
            .Where(o => o.CreatedAt >= dayStart && o.CreatedAt < dayEnd)
            .SumAsync(o => o.TotalPrice);
        return new List<SalesStatPoint> { new SalesStatPoint { PeriodStart = dayStart, TotalSales = total } };
    }

    public async Task<List<SalesStatPoint>> GetStatsByWeekAsync(DateTime weekStart, DateTime weekEnd)
    {
        var start = DateTime.SpecifyKind(weekStart.Date, DateTimeKind.Utc);
        var end = DateTime.SpecifyKind(weekEnd.Date, DateTimeKind.Utc).AddDays(1);
        var total = await _db.Orders.AsNoTracking()
            .Where(o => o.CreatedAt >= start && o.CreatedAt < end)
            .SumAsync(o => o.TotalPrice);
        return new List<SalesStatPoint> { new SalesStatPoint { PeriodStart = start, TotalSales = total } };
    }

    public async Task<List<SalesStatPoint>> GetStatsByMonthAsync(int year, int month)
    {
        var start = DateTime.SpecifyKind(new DateTime(year, month, 1), DateTimeKind.Utc);
        var end = start.AddMonths(1);
        var total = await _db.Orders.AsNoTracking()
            .Where(o => o.CreatedAt >= start && o.CreatedAt < end)
            .SumAsync(o => o.TotalPrice);
        return new List<SalesStatPoint> { new SalesStatPoint { PeriodStart = start, TotalSales = total } };
    }
}
