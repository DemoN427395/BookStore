using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Services;

public class BookManager
{
    private readonly UserDataContext _dbContext;
    private readonly AuthServiceClient _authServiceClient;

    public BookManager(UserDataContext dbContext, AuthServiceClient authServiceClient)
    {
        _dbContext = dbContext;
        _authServiceClient = authServiceClient;
    }

    public async Task ProcessBookAsync(int bookId)
    {
        // Получаем книгу из базы данных
        var book = await _dbContext.Books.FirstOrDefaultAsync(b => b.Id == bookId);
        if (book == null)
        {
            // Обработка ошибки: книга не найдена
            return;
        }

        // Вызываем Auth-сервис для получения данных о пользователе
        var user = await _authServiceClient.GetUserByIdAsync();
        if (user != null)
        {
            // Дальнейшая обработка: например, логирование или отправка уведомления
            Console.WriteLine($"Книга '{book.Title}' принадлежит пользователю {user.Name}");
        }
        else
        {
            // Обработка ситуации, когда пользователь не найден в Auth-сервисе
            Console.WriteLine("Пользователь не найден");
        }
    }
}
