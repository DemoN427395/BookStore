﻿using Microsoft.EntityFrameworkCore;
using BookStoreLib.Models;

namespace BookStoreLib.Data;

// DbContext for the User database, managing books
public class BooksDbContext : DbContext
{
    public BooksDbContext(DbContextOptions<BooksDbContext> options) : base(options) { }

    public DbSet<BookModel> Books { get; set; }

    // Configuring the model for Book entities
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("books");

        // Configure BookModel entity
        modelBuilder.Entity<BookModel>(entity =>
        {
            entity.ToTable("Books");
            entity.HasKey(b => b.Id);
            entity.Property(b => b.UserId).IsRequired();
            entity.HasIndex(b => b.Title).IsUnique();
            entity.HasIndex(b => b.ISBN).IsUnique();
        });

        // Configure ApplicationUser entity for AspNetUsers (if needed)
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("AspNetUsers", "user", t => t.ExcludeFromMigrations())
                .HasNoKey();
        });
    }

    public DbSet<ApplicationUser> AspNetUsers { get; set; }
}
