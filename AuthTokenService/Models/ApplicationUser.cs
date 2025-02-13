using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace AuthTokenService.Models;

public class ApplicationUser : IdentityUser
{ 
    public string Name { get; set; } = string.Empty;
}