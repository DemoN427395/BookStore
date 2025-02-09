﻿using UserService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthTokenService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BooksController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBook(BookModel book)
        {
            try
            {
                bool bookExists = await _context.BooksModels
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
                    Language = book.Language,
                    CoverImage = !string.IsNullOrEmpty(book.CoverImageBase64)
                        ? Convert.FromBase64String(book.CoverImageBase64)
                        : null
                };

                _context.BooksModels.Add(newBook);
                await _context.SaveChangesAsync();
                return Ok($"Successfully Added with Id {newBook.Id}");
            }
            catch (Exception ex)
            {
                return BadRequest("An error occurred while adding the book.");
            }
        }
    }
}