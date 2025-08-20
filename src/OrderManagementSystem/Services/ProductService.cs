using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Data;
using OrderManagementSystem.DTOs;
using OrderManagementSystem.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace OrderManagementSystem.Services;

public class ProductService
{
    private readonly AppDbContext _db;
    private readonly IDistributedCache _cache;
    public ProductService(AppDbContext db, IDistributedCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<ProductDto> CreateAsync(CreateProductRequest req)
    {
        var entity = new Product { Name = req.Name, Price = req.Price, Stock = req.Stock };
        _db.Products.Add(entity);
        await _db.SaveChangesAsync();
        return Map(entity);
    }

    public async Task<ProductDto?> GetAsync(Guid id)
    {
        var e = await _db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return e is null ? null : Map(e);
    }

    public async Task<List<ProductDto>> GetAllAsync()
    {
        var cacheKey = "product_list";
        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached != null)
            return JsonSerializer.Deserialize<List<ProductDto>>(cached) ?? new();
        var products = await _db.Products.AsNoTracking().Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Stock = p.Stock
        }).ToListAsync();
        var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30) };
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(products), options);
        return products;
    }

    public async Task<ProductDto?> UpdateAsync(Guid id, UpdateProductRequest req)
    {
        var e = await _db.Products.FirstOrDefaultAsync(x => x.Id == id);
        if (e is null) return null;
        e.Name = req.Name;
        e.Price = req.Price;
        e.Stock = req.Stock;
        await _db.SaveChangesAsync();
        return Map(e);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var e = await _db.Products.FirstOrDefaultAsync(x => x.Id == id);
        if (e is null) return false;
        _db.Products.Remove(e);
        await _db.SaveChangesAsync();
        return true;
    }

    private static ProductDto Map(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Price = p.Price,
        Stock = p.Stock
    };
}
