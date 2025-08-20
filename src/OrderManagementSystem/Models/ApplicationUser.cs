using Microsoft.AspNetCore.Identity;

namespace OrderManagementSystem.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public UserRole Role { get; set; } = UserRole.User;
}
