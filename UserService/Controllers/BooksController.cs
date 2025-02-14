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
public class BooksController : ControllerBase
{
    private readonly UserDbContext _context;

    public BooksController(UserDbContext context)
    {
        _context = context;
    }

    [HttpPost("create")]
    [Authorize]
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

    [HttpGet]
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