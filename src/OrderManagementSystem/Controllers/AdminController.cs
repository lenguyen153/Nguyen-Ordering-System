using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.DTOs;
using OrderManagementSystem.Services;

namespace OrderManagementSystem.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly AdminService _svc;
    public AdminController(AdminService svc) => _svc = svc;

    [HttpGet("orders")]
    public Task<List<OrderDto>> AllOrders() => _svc.GetAllOrdersAsync();

    [HttpGet("stats")]
    public async Task<IActionResult> Stats(
        [FromQuery] SalesRange range,
        [FromQuery] DateTime? date = null,
        [FromQuery] DateTime? weekStart = null,
        [FromQuery] DateTime? weekEnd = null,
        [FromQuery] int? month = null,
        [FromQuery] int? year = null)
    {
        int provided = 0;
        if (date != null) provided++;
        if (weekStart != null || weekEnd != null) provided++;
        if (month != null || year != null) provided++;
        if (provided != 1)
            return BadRequest("Provide only one of: date, weekStart/weekEnd, or month/year.");

        try
        {
            switch (range)
            {
                case SalesRange.Day:
                    if (date == null)
                        return BadRequest("For 'day' range, 'date' parameter is required (yyyy-MM-dd).");
                    return Ok(await _svc.GetStatsByDayAsync(date.Value));
                case SalesRange.Week:
                    if (weekStart == null || weekEnd == null)
                        return BadRequest("For 'week' range, both 'weekStart' and 'weekEnd' parameters are required (yyyy-MM-dd).");
                    if (weekEnd < weekStart)
                        return BadRequest("'weekEnd' must be after 'weekStart'.");
                    return Ok(await _svc.GetStatsByWeekAsync(weekStart.Value, weekEnd.Value));
                case SalesRange.Month:
                    if (month == null || year == null)
                        return BadRequest("For 'month' range, both 'month' (1-12) and 'year' are required.");
                    if (month < 1 || month > 12)
                        return BadRequest("'month' must be between 1 and 12.");
                    return Ok(await _svc.GetStatsByMonthAsync(year.Value, month.Value));
                default:
                    return BadRequest("Invalid range.");
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
