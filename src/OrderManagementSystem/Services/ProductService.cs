using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Data;
using OrderManagementSystem.DTOs;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Services;

public class ProductService
{
    private readonly AppDbContext _db;
    public ProductService(AppDbContext db) => _db = db;

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

    public Task<List<ProductDto>> GetAllAsync()
        => _db.Products.AsNoTracking().Select(p => Map(p)).ToListAsync();

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
