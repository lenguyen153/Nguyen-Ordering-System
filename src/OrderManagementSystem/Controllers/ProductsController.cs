using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.DTOs;
using OrderManagementSystem.Services;

namespace OrderManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _svc;
    public ProductsController(ProductService svc) => _svc = svc;

    [HttpGet]
    [Authorize(Roles = "Admin,User")]
    public Task<List<ProductDto>> GetAll() => _svc.GetAllAsync();

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<ActionResult<ProductDto>> Get(Guid id)
    {
        var p = await _svc.GetAsync(id);
        return p is null ? NotFound() : Ok(p);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public Task<ProductDto> Create(CreateProductRequest req) => _svc.CreateAsync(req);

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> Update(Guid id, UpdateProductRequest req)
    {
        var p = await _svc.UpdateAsync(id, req);
        return p is null ? NotFound() : Ok(p);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var ok = await _svc.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }
}
