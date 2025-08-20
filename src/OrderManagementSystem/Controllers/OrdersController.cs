using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.DTOs;
using OrderManagementSystem.Services;

namespace OrderManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "User,Admin")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _svc;
    public OrdersController(OrderService svc) => _svc = svc;

    [HttpPost]
    public Task<OrderDto> Create(CreateOrderRequest req) => _svc.CreateOrderAsync(req);

    [HttpGet("my")]
    public Task<List<OrderDto>> My() => _svc.GetMyOrdersAsync();
}
