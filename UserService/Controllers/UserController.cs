// Controllers/UserController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BookStoreLib.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using UserService.Services;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly HttpClient _authServiceClient;
    private readonly BookManager _bookManager;

    public UserController(IHttpClientFactory httpClientFactory, BookManager bookManager)
    {
        _authServiceClient = httpClientFactory.CreateClient("AuthTokenService");
        _bookManager = bookManager;
    }


    [HttpGet("process-book/{bookId}")]
    public async Task<IActionResult> ProcessBook(int bookId)
    {
        await _bookManager.ProcessBookAsync(bookId);
        return Ok();
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        // Пересылаем токен в AuthTokenService
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        _authServiceClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _authServiceClient.GetAsync("/api/users/me");
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode);
        }

        var userData = await response.Content.ReadFromJsonAsync<UserData>();
        return Ok(userData);
    }
}
    // [HttpGet("me")]
    // public async Task<IActionResult> GetCurrentUserId()
    // {
    //     try
    //     {
    //         // Получаем ID пользователя из Claims
    //         var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    //
    //         if (string.IsNullOrEmpty(userId))
    //         {
    //             return Unauthorized(new { message = "User ID not found in claims." });
    //         }
    //
    //         // Ищем пользователя в базе данных
    //         var user = await _context.Users.FindAsync(userId);
    //
    //         if (user == null)
    //         {
    //             return NotFound(new { message = "User not found." });
    //         }
    //
    //         return Ok(new
    //         {
    //             Id = user.Id,
    //             Email = user.Email,
    //             Name = user.Name
    //         });
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
    //     }
    // }
