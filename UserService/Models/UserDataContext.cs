using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserService.DTOs;

namespace UserService.Models;

public class UserDataContext : IdentityDbContext<ApplicationUser>
{
    public UserDataContext(DbContextOptions<UserDataContext> options)
        : base(options)
    {
    }

    public DbSet<BookModel> Books { get; set; }
}
