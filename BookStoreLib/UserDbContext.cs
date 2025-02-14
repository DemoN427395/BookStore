using Microsoft.EntityFrameworkCore;
using BookStoreLib.Models;

namespace BookStoreLib.Data;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

    public DbSet<BookModel> Books { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("user");

        // Настройка Books
        modelBuilder.Entity<BookModel>(entity =>
        {
            entity.ToTable("Books");
            entity.HasKey(b => b.Id);
            entity.Property(b => b.UserId).IsRequired();
            entity.HasIndex(b => b.Title).IsUnique();
            entity.HasIndex(b => b.ISBN).IsUnique();

        });

        // Настройка для чтения AspNetUsers (если требуется)
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("AspNetUsers", "auth", t => t.ExcludeFromMigrations())
                .HasNoKey();
        });
    }

    public DbSet<ApplicationUser> AspNetUsers { get; set; }
}