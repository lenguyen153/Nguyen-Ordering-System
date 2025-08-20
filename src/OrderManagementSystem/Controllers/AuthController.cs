using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Auth;
using OrderManagementSystem.DTOs;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JwtTokenService _jwtService;

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, JwtTokenService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest req)
    {
        if (req.Role is not ("Admin" or "User"))
            return BadRequest("Role must be Admin or User.");
        var user = new ApplicationUser { UserName = req.Email, Email = req.Email, Role = req.Role == "Admin" ? UserRole.Admin : UserRole.User };
        var result = await _userManager.CreateAsync(user, req.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);
        await _userManager.AddToRoleAsync(user, req.Role);
        var token = _jwtService.GenerateToken(user, new[] { req.Role });
        return Ok(new AuthResponse { Token = token });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest req)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user is null) return Unauthorized();
        var result = await _signInManager.CheckPasswordSignInAsync(user, req.Password, false);
        if (!result.Succeeded) return Unauthorized();
        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtService.GenerateToken(user, roles);
        return Ok(new AuthResponse { Token = token });
    }
}
