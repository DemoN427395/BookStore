using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BookStoreLib.Models;

namespace BookStoreLib.Data;

public class AuthDbContext : IdentityDbContext<ApplicationUser>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    public DbSet<TokenInfo> TokenInfos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("auth");

        // Настройка для ApplicationUser
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("AspNetUsers");
            // Добавьте дополнительные настройки для новых полей
            entity.Property(u => u.Name).HasMaxLength(100);
        });

        // Настройка для TokenInfo
        modelBuilder.Entity<TokenInfo>()
            .ToTable("TokenInfos");
    }
}