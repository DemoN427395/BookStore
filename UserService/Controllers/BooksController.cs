using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using UserService.Models;

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
