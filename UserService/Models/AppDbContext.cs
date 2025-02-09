using UserService.Models;
using Microsoft.EntityFrameworkCore;

namespace UserService.Models;

public class AppDbContext
{
    public DbSet<BookModel> BooksModels { get; set; }
}