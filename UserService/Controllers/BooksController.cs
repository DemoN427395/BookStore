using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using UserService.Models;
using System.Security.Claims;

namespace UserService.Controllers;
    [ApiController]
    [Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly UserDataContext _context;

    public BooksController(UserDataContext context)
    {
        _context = context;
    }

    // [HttpPost("create")]
    // [Authorize]
    // public async Task<IActionResult> CreateBook(BookModel book)
    // {
    //     try
    //     {
    //         bool bookExists = await _context.Books
    //             .AnyAsync(b => b.Title == book.Title);
    //
    //         if (bookExists)
    //         {
    //             return BadRequest("Book already exists");
    //         }
    //
    //         BookModel newBook = new()
    //         {
    //             Title = book.Title,
    //             Author = book.Author,
    //             Genre = book.Genre,
    //             Year = book.Year,
    //             Publisher = book.Publisher,
    //             ISBN = book.ISBN,
    //             Pages = book.Pages,
    //             Language = book.Language
    //             // CoverImage = !string.IsNullOrEmpty(book.CoverImageBase64)
    //             //     ? Convert.FromBase64String(book.CoverImageBase64)
    //             //     : null
    //         };
    //
    //         _context.Books.Add(newBook);
    //         await _context.SaveChangesAsync();
    //         return Ok($"Successfully Added with Id {newBook.Id}");
    //     }
    //     catch (Exception ex)
    //     {
    //         return BadRequest("An error occurred while adding the book.");
    //     }
    // }

    // UserService/Controllers/BooksController.cs
    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> CreateBook(BookModel book)
    {
        try
        {
            // Извлекаем UserId из токена
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            // Привязываем книгу к пользователю
            book.UserId = userId;

            // Проверка на дубликат (для текущего пользователя)
            bool bookExists = await _context.Books
                .AnyAsync(b => b.Title == book.Title && b.UserId == userId);

            if (bookExists)
            {
                return BadRequest("Book already exists");
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return Ok($"Successfully Added with Id {book.Id}");
        }
        catch (Exception ex)
        {
            return BadRequest("An error occurred while adding the book.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRandomBooks()
    {
        try
        {
            return Ok(await _context.Books
                .OrderBy(b => Guid.NewGuid())
                .Take(5)
                .ToListAsync());
        }
        catch (Exception ex)
        {
            return BadRequest("An error occurred while fetching the books.");
        }
    }
}
