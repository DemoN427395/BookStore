// Controllers/BooksController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BookStoreLib.Models;
using System.Security.Claims;
using BookStoreLib.Data;
using BookStoreLib.DTOs;
using System.Net.Mime;

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
    public async Task<IActionResult> CreateBook([FromForm] CreateBookDTO dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Проверка существования пользователя
        var userExists = await _context.AspNetUsers.AnyAsync(u => u.Id == userId);
        if (!userExists)
            return BadRequest("User does not exist.");

        if (await _context.Books.AnyAsync(b => b.Title == dto.Title))
            return BadRequest("Book with this name already exists");

        if (await _context.Books.AnyAsync(b => b.ISBN == dto.ISBN))
            return BadRequest("Book with this ISBN already exists");

        byte[]? fileContent = null;
        string? contentType = null;

        if (dto.File != null)
        {
            using (var memoryStream = new MemoryStream())
            {
                await dto.File.CopyToAsync(memoryStream);
                fileContent = memoryStream.ToArray();
                contentType = dto.File.ContentType;
            }
        }

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
            UserId = userId,
            FileContent = fileContent,
            ContentType = contentType
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

    [AllowAnonymous]
    [HttpGet("download/id={id}")]
    public async Task<IActionResult> DownloadBookFile([FromRoute] int id)
    {
        try
        {
            // Поиск книги в БД
            var book = await _context.Books
                .Where(b => b.Id == id)
                .Select(b => new { b.Title, b.FileContent, b.ContentType })
                .FirstOrDefaultAsync();

            if (book == null)
                return NotFound($"Book with ID {id} not found.");

            if (book.FileContent == null || book.FileContent.Length == 0)
                return NotFound($"File for book with ID {id} not found.");

            // Определение имени файла
            var fileName = string.IsNullOrWhiteSpace(book.Title) ? $"book_{id}.pdf" : $"{book.Title}.pdf";
            var contentType = string.IsNullOrWhiteSpace(book.ContentType) ? "application/octet-stream" : book.ContentType;

            // Возвращаем файл
            return File(book.FileContent, contentType, fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
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
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.Author,
                    b.Genre,
                    b.Year,
                    b.Publisher,
                    b.ISBN,
                    b.Pages,
                    b.Language,
                    b.UserId
                })
                .ToListAsync();

            return Ok(books);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

}