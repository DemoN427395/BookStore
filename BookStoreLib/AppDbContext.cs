using Microsoft.EntityFrameworkCore;
using BookStoreLib.Models;
using Microsoft.AspNetCore.Identity;

namespace BookStoreLib.Data;

public class AuthDbContext : BaseDbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    public DbSet<TokenInfo> TokenInfos { get; set; }

    // AuthDbContext.cs
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("auth");

        modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers", "auth");
        modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles", "auth");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles", "auth");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims", "auth");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins", "auth");
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens", "auth");
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims", "auth");

        modelBuilder.Entity<TokenInfo>().ToTable("TokenInfos", "auth");
        base.OnModelCreating(modelBuilder);
    }
}