﻿using Microsoft.AspNetCore.Mvc;
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
    private readonly BooksDbContext _context;

    public BooksController(BooksDbContext context)
    {
        _context = context;
    }

    // Create a new book
    [HttpPost("create")]
    public async Task<IActionResult> CreateBook([FromForm] CreateBookDTO dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var userExists = await _context.AspNetUsers.AnyAsync(u => u.Id == userId);
        if (!userExists)
            return BadRequest(new { message = "User does not exist." });

        if (await _context.Books.AnyAsync(b => b.Title == dto.Title))
            return BadRequest(new { message = "Book with this name already exists" });

        if (await _context.Books.AnyAsync(b => b.ISBN == dto.ISBN))
            return BadRequest(new { message = "Book with this ISBN already exists" });

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
        // return Ok(new { bookId = newBook.Id });
        return Ok(newBook);
    }


    [HttpPatch("update")]
    public async Task<IActionResult> UpdateBook([FromForm] UpdateBookDTO dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!await _context.AspNetUsers.AnyAsync(u => u.Id == userId))
            return Unauthorized("User is not authorized.");

        var book = await _context.Books
            .FirstOrDefaultAsync(b => b.Id == dto.Id && b.UserId == userId);
        if (book == null)
            return NotFound("Book not found or access denied.");

        // Check for unique title
        if (dto.Title != null && dto.Title != book.Title)
        {
            if (await _context.Books.AnyAsync(b => b.Title == dto.Title && b.Id != book.Id))
                return BadRequest("A book with this title already exists.");
        }

        // Check for unique ISBN
        if (dto.ISBN != null && dto.ISBN != book.ISBN)
        {
            if (await _context.Books.AnyAsync(b => b.ISBN == dto.ISBN && b.Id != book.Id))
                return BadRequest("A book with this ISBN already exists.");
        }

        // Update the file if provided
        if (dto.File != null)
        {
            using var memoryStream = new MemoryStream();
            await dto.File.CopyToAsync(memoryStream);
            book.FileContent = memoryStream.ToArray();
            book.ContentType = dto.File.ContentType;
        }

        // Update other fields
        book.Title = dto.Title ?? book.Title;
        book.Author = dto.Author ?? book.Author;
        book.Genre = dto.Genre ?? book.Genre;
        book.Year = dto.Year ?? book.Year;
        book.Publisher = dto.Publisher ?? book.Publisher;
        book.ISBN = dto.ISBN ?? book.ISBN;
        book.Pages = dto.Pages ?? book.Pages;
        book.Language = dto.Language ?? book.Language;

        try
        {
            await _context.SaveChangesAsync();
            return Ok(book);
        }
        catch (DbUpdateException ex)
        {
            return BadRequest($"Save error: {ex.Message}");
        }
    }


    // Delete a book
    [HttpDelete("delete/id={id}")]
    public async Task<IActionResult> DeleteBook([FromRoute] int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "User is unauthorized" });

        var userExists = await _context.AspNetUsers.AnyAsync(u => u.Id == userId);
        if (!userExists)
            return Unauthorized(new { message = "User does not exist" });

        // var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == dto.Id && b.UserId == userId);
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
        if (book == null)
            return NotFound(new { message = $"Book with id {id} not found" });


        try
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Book deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
        }
    }

    // Download book file by ID
    // [AllowAnonymous]
    [HttpGet("download/id={id}")]
    public async Task<IActionResult> DownloadBookFile([FromRoute] int id)
    {
        try
        {
            var book = await _context.Books
                .Where(b => b.Id == id)
                .Select(b => new { b.Title, b.FileContent, b.ContentType })
                .FirstOrDefaultAsync();

            if (book == null)
                return NotFound(new { message = $"Book with ID {id} not found." });

            if (book.FileContent == null || book.FileContent.Length == 0)
                return NotFound(new { message = $"File for book with ID {id} not found." });

            var fileName = string.IsNullOrWhiteSpace(book.Title) ? $"book_{id}.pdf" : $"{book.Title}.pdf";
            var contentType = string.IsNullOrWhiteSpace(book.ContentType) ? "application/octet-stream" : book.ContentType;

            return File(book.FileContent, contentType, fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
        }
    }

    // Get random books
    [AllowAnonymous]
    [HttpGet]
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
            return BadRequest(new { message = ex.Message });
        }
    }

    // Search book
    [AllowAnonymous]
    [HttpGet("search/id={id}")]
    public async Task<IActionResult> SearchBook([FromRoute] int id)
    {
        try
        {
            var book = await _context.Books
                .Where(b => b.Id == id)
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
                .FirstOrDefaultAsync();

            if (book == null)
                return NotFound(new { message = $"Book with ID {id} not found." });

            return Ok(book);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
