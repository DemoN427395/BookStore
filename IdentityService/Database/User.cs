using Microsoft.AspNetCore.Identity;

namespace IdentityService.Database;

public class User : IdentityUser
{
    public string? Initials { get; set; }

}