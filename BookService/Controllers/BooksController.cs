using System.Security.Claims;
using BookService.Classes;
using BookService.Interfaces;
using BookStoreLib.Data;
using BookStoreLib.DTOs;
using BookStoreLib.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookService.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize]
[RequestSizeLimit(int.MaxValue)]
public class BooksController : ControllerBase, IBooksController
{
    private readonly HashManagerClass _hashManagerClass = new HashManagerClass();

    private readonly ILogger<BooksController> _logger;
    private readonly BooksDbContext _context;

    private string? _fileHash = null;
    private string? _coverFileHash = null;
    // private readonly string _storagePath;

    public string StoragePath { get; }
    public string BooksPath { get; }
    public string BookCoversPath { get; }

    public BookModel DeleteBookModel { get; private set; }

    public BooksController(BooksDbContext context, ILogger<BooksController> logger)
    {
        _context = context;
        _logger = logger;

        StoragePath = Path.Combine(Directory.GetCurrentDirectory(), "MediaFiles");
        Directory.CreateDirectory(StoragePath);

        BooksPath = Path.Combine(StoragePath, "Books");
        Directory.CreateDirectory(BooksPath);

        BookCoversPath = Path.Combine(StoragePath, "Book covers");
        Directory.CreateDirectory(BookCoversPath);
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

        string? filePath = null;
        string? coverFilePath = null;
        string? contentType = null;

        if (dto.File != null)
        {

            using (var stream = new MemoryStream())
            {
                await dto.File.CopyToAsync(stream);
                _fileHash = await _hashManagerClass.ComputeHashFromMemoryStreamAsync(stream);
            }

            // Генерируем уникальное имя файла с сохранением оригинального расширения
            var uniqueFileName = $"{_fileHash}{Path.GetExtension(dto.File.FileName)}";
            var fullFilePath = Path.Combine(BooksPath, uniqueFileName);

            // Сохраняем файл на диск
            using (var stream = new FileStream(fullFilePath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            filePath = uniqueFileName;
            contentType = dto.File.ContentType;
        }

        if (dto.CoverFile != null)
        {
            using (var stream = new MemoryStream())
            {
                await dto.CoverFile.CopyToAsync(stream);
                _coverFileHash = await _hashManagerClass.ComputeHashFromMemoryStreamAsync(stream);
            }

            var uniqueCoverFileName = $"{_coverFileHash}{Path.GetExtension(dto.CoverFile.FileName)}";
            var fullCoverFilePath = Path.Combine(BookCoversPath, uniqueCoverFileName);
            using (var stream = new FileStream(fullCoverFilePath, FileMode.Create))
            {
                await dto.CoverFile.CopyToAsync(stream);
            }
            coverFilePath = uniqueCoverFileName;
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
            FilePath = filePath,
            CoverFilePath = coverFilePath,
            ContentType = contentType
        };

        _context.Books.Add(newBook);
        await _context.SaveChangesAsync();

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

        // Проверка на уникальность заголовка
        if (dto.Title != null && dto.Title != book.Title)
        {
            if (await _context.Books.AnyAsync(b => b.Title == dto.Title && b.Id != book.Id))
                return BadRequest("A book with this title already exists.");
        }

        // Проверка на уникальность ISBN
        if (dto.ISBN != null && dto.ISBN != book.ISBN)
        {
            if (await _context.Books.AnyAsync(b => b.ISBN == dto.ISBN && b.Id != book.Id))
                return BadRequest("A book with this ISBN already exists.");
        }

        // Если передан новый файл – обновляем медиа
        if (dto.File != null)
        {
            // Если ранее был сохранён файл, удаляем его
            if (!string.IsNullOrEmpty(book.FilePath))
            {
                var oldFilePath = Path.Combine(BooksPath, book.FilePath);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            using (var stream = new MemoryStream())
            {
                await dto.File.CopyToAsync(stream);
                _fileHash = await _hashManagerClass.ComputeHashFromMemoryStreamAsync(stream);
            }

            // Генерируем уникальное имя файла с сохранением оригинального расширения
            var uniqueFileName = $"{_fileHash}{Path.GetExtension(dto.File.FileName)}";
            var fullFilePath = Path.Combine(BooksPath, uniqueFileName);
            
            using (var stream = new FileStream(fullFilePath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            book.FilePath = uniqueFileName;
            book.ContentType = dto.File.ContentType;
        }

        if (dto.CoverFile != null)
        {
            // Если ранее был сохранён файл, удаляем его
            if (!string.IsNullOrEmpty(book.CoverFilePath))
            {
                var oldCoverFilePath = Path.Combine(BookCoversPath, book.CoverFilePath);
                if (System.IO.File.Exists(oldCoverFilePath))
                {
                    System.IO.File.Delete(oldCoverFilePath);
                }
            }
            using (var stream = new MemoryStream())
            {
                await dto.CoverFile.CopyToAsync(stream);
                _coverFileHash = await _hashManagerClass.ComputeHashFromMemoryStreamAsync(stream);
            }
            var uniqueCoverFileName = $"{_coverFileHash}{Path.GetExtension(dto.CoverFile.FileName)}";
            var fullCoverFilePath = Path.Combine(BookCoversPath, uniqueCoverFileName);

            using (var stream = new FileStream(fullCoverFilePath, FileMode.Create))
            {
                await dto.CoverFile.CopyToAsync(stream);
            }
            book.CoverFilePath = uniqueCoverFileName;
        }

        // Обновляем остальные поля
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
            DeleteBookModel = book;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            // Удаляем файл с диска, если он существует
            using (var stream = new MemoryStream())
            {
                if (!string.IsNullOrEmpty(book.FilePath))
                {
                    var filePath = Path.Combine(BooksPath, book.FilePath);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
                if (!string.IsNullOrEmpty(book.CoverFilePath))
                {
                    var coverFilePath = Path.Combine(BookCoversPath, book.CoverFilePath);
                    if (System.IO.File.Exists(coverFilePath))
                    {
                        System.IO.File.Delete(coverFilePath);
                    }
                }
            }
            return Ok(new { message = "Book deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
        }
    }

    // Download book file by ID
    [AllowAnonymous]
    [HttpGet("download/id={id}")]
    public async Task<IActionResult> DownloadBookFile([FromRoute] int id)
    {
        try
        {
            var book = await _context.Books
                .Where(b => b.Id == id)
                .Select(b => new { b.Title, b.FilePath, b.ContentType })
                .FirstOrDefaultAsync();

            if (book == null)
                return NotFound(new { message = $"Book with ID {id} not found." });

            if (string.IsNullOrEmpty(book.FilePath))
                return NotFound(new { message = $"File for book with ID {id} not found." });

            var fullFilePath = Path.Combine(BooksPath, book.FilePath);
            if (!System.IO.File.Exists(fullFilePath))
                return NotFound(new { message = $"File for book with ID {id} not found on disk." });

            var fileName = string.IsNullOrWhiteSpace(book.Title) ? $"book_{id}.pdf" : $"{book.Title}.pdf";
            var contentType = string.IsNullOrWhiteSpace(book.ContentType) ? "application/octet-stream" : book.ContentType;

            var fileBytes = await System.IO.File.ReadAllBytesAsync(fullFilePath);
            return File(fileBytes, contentType, fileName);
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