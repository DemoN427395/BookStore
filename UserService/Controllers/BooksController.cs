// Controllers/BooksController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BookStoreLib.Models;
using System.Security.Claims;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly UserDbContext _context;

    public BooksController(UserDbContext context)
    {
        _context = context;
    }

    // [HttpPost("create")]
    // [Authorize]
    // public async Task<IActionResult> CreateBook(BookModel book)
    // {
    //     var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    //     if (string.IsNullOrEmpty(userId)) return Unauthorized();
    //
    //     book.UserId = userId;
    //
    //     if (await _context.Books.AnyAsync(b => b.Title == book.Title && b.UserId == userId))
    //         return BadRequest("Book exists");
    //
    //     _context.Books.Add(book);
    //     await _context.SaveChangesAsync();
    //     return Ok($"Book created: {book.Id}");
    // }

    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> CreateBook(BookModel book)
    {
        try
        {
            bool bookExists = await _context.Books
                .AnyAsync(b => b.Title == book.Title);
    
            if (bookExists)
            {
                return BadRequest("Book already exists");
            }
    
            BookModel newBook = new()
            {
                Title = book.Title,
                Author = book.Author,
                Genre = book.Genre,
                Year = book.Year,
                Publisher = book.Publisher,
                ISBN = book.ISBN,
                Pages = book.Pages,
                Language = book.Language
                // CoverImage = !string.IsNullOrEmpty(book.CoverImageBase64)
                //     ? Convert.FromBase64String(book.CoverImageBase64)
                //     : null
            };
    
            _context.Books.Add(newBook);
            await _context.SaveChangesAsync();
            return Ok($"Successfully Added with Id {newBook.Id}");
        }
        catch (Exception ex)
        {
            return BadRequest("An error occurred while adding the book.");
        }
    }


    [HttpGet]
    public async Task<IActionResult> GetRandomBooks()
    {
        var books = await _context.Books
            .OrderBy(b => Guid.NewGuid())
            .Take(5)
            .ToListAsync();

        return Ok(books);
    }
}