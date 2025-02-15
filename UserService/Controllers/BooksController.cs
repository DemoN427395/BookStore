// Controllers/BooksController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BookStoreLib.Models;
using System.Security.Claims;
using BookStoreLib.Data;
using BookStoreLib.DTOs;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BooksController : ControllerBase
{
    private readonly UserDbContext _context;

    public BooksController(UserDbContext context)
    {
        _context = context;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateBook([FromBody] CreateBookDTO dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Проверка существования пользователя (только чтение)
        var userExists = await _context.AspNetUsers
            .AnyAsync(u => u.Id == userId);

        if (!userExists)
            return BadRequest("User does not exist.");

        if (await _context.Books.AnyAsync(b => b.Title == dto.Title))
            return BadRequest("Book with this name exist");

        if (await _context.Books.AnyAsync(b => b.ISBN == dto.ISBN))
            return BadRequest("Book with this ISBN exist");

        var newBook = new BookModel
        {
            Title = dto.Title,
            Author = dto.Author,
            Genre = dto.Genre,
            Year = dto.Year,
            Publisher = dto.Publisher,
            ISBN = dto.ISBN,
            Pages = dto.Pages,
            Language = dto.Language,
            UserId = userId
        };

        _context.Books.Add(newBook);
        await _context.SaveChangesAsync();
        return Ok(newBook.Id);
    }

    [HttpPatch("update")]
    public async Task<IActionResult> UpdateBook([FromBody] UpdateBookDTO dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // 1. Проверка существования пользователя
        var userExists = await _context.AspNetUsers.AnyAsync(u => u.Id == userId);
        if (!userExists) return Unauthorized();

        // 2. Находим книгу по ID из DTO
        var book = await _context.Books
            .FirstOrDefaultAsync(b => b.Id == dto.Id && b.UserId == userId);

        if (book == null)
            return NotFound("Book not found or access denied");

        // 3. Обновляем только изменяемые поля
        book.Title = dto.Title ?? book.Title;
        book.Author = dto.Author ?? book.Author;
        book.Genre = dto.Genre ?? book.Genre;
        book.Year = dto.Year ?? book.Year;
        book.Publisher = dto.Publisher ?? book.Publisher;
        book.ISBN = dto.ISBN ?? book.ISBN;
        book.Pages = dto.Pages ?? book.Pages;
        book.Language = dto.Language ?? book.Language;

        // 4. Сохраняем изменения
        try
        {
            await _context.SaveChangesAsync();
            return Ok(book);
        }
        catch (DbUpdateException ex)
        {
            return BadRequest($"Update failed: {ex.Message}");
        }
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteBook([FromBody] DeleteBookDTO dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        // Проверка существования пользователя
        var userExists = await _context.AspNetUsers.AnyAsync(u => u.Id == userId);
        if (!userExists)
            return Unauthorized();

        // Поиск книги по ID и UserId
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == dto.Id && b.UserId == userId);
        if (book == null)
            return NotFound("Book not found or access denied");

        try
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return Ok("Book deleted successfully");
        }
        catch (Exception ex)
        {
            // Здесь можно добавить логирование ошибки ex
            return StatusCode(500, "An error occurred while deleting the book.");
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetRandomBooks()
    {
        try
        {
            var books = await _context.Books
                .OrderBy(b => Guid.NewGuid())
                .Take(5)
                .ToListAsync();

            return Ok(books);
        }
        catch (Exception ex)
        {
            return (BadRequest(ex));
        }
    }
}