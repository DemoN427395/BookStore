using Microsoft.AspNetCore.Identity;

namespace BookStoreLib.Models;

public class ApplicationUser : IdentityUser
{ 
    public string Name { get; set; } = string.Empty;
}