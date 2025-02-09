using Microsoft.AspNetCore.Identity;

namespace AuthTokenService.Models;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; } = string.Empty;
}