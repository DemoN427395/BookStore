using BookStoreLib.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class UserDbContext : BaseDbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

    public DbSet<BookModel> Books { get; set; } // Убедитесь, что модель называется Book

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("user");

        // Настройка таблиц Identity
        modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers", "user");
        modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles", "user");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles", "user");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims", "user");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins", "user");
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens", "user");
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims", "user");

        modelBuilder.Entity<BookModel>()
            .ToTable("Books", "user")
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(b => b.UserId);

        base.OnModelCreating(modelBuilder);
    }
}