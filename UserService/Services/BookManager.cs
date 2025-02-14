// Services/BookManager.cs
using Microsoft.EntityFrameworkCore;
using BookStoreLib.Models;

namespace UserService.Services
{
    public class BookManager
    {
        private readonly UserDbContext _dbContext;
        private readonly AuthServiceClient _authServiceClient;

        // Добавляем AuthServiceClient в конструктор
        public BookManager(
            UserDbContext dbContext,
            AuthServiceClient authServiceClient)
        {
            _dbContext = dbContext;
            _authServiceClient = authServiceClient;
        }

        public async Task ProcessBookAsync(int bookId)
        {
            var book = await _dbContext.Books
                .FirstOrDefaultAsync(b => b.Id == bookId);

            if (book == null) return;

            var user = await _authServiceClient.GetUserByIdAsync(book.UserId);
            Console.WriteLine(user != null
                ? $"Книга '{book.Title}' принадлежит {user.Name}"
                : "Пользователь не найден");
        }
    }
}