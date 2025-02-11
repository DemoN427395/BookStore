using Microsoft.EntityFrameworkCore;

namespace UserService.Models
{
    public class UserDataContext : DbContext
    {
        public UserDataContext(DbContextOptions<UserDataContext> options)
            : base(options)
        {
        }

        public DbSet<BookModel> Books { get; set; }
    }
}
