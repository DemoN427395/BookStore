// Services/BookManager.cs
using Microsoft.EntityFrameworkCore;
using BookStoreLib.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using BookStoreLib.Data;

namespace UserService.Services
{
    public class BookManager
    {
        private readonly UserDbContext _dbContext;
        private readonly AuthServiceClient _authServiceClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BookManager(
            UserDbContext dbContext,
            AuthServiceClient authServiceClient,
            IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _authServiceClient = authServiceClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task ProcessBookAsync(int bookId)
        {
            var book = await _dbContext.Books
                .FirstOrDefaultAsync(b => b.Id == bookId);

            if (book == null) return;

            var accessToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                .ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(accessToken))
            {
                Console.WriteLine("Access token not found");
                return;
            }

            var user = await _authServiceClient.GetCurrentUserAsync(accessToken);
            Console.WriteLine(user != null
                ? $"Книга '{book.Title}' принадлежит {user.UserName}"
                : "Пользователь не найден");
        }
    }
}