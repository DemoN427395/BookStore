// Controllers/UserController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BookStoreLib.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly UserDbContext _context;

    public UserController(UserManager<ApplicationUser> userManager, UserDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
    
        var user = await _userManager.FindByIdAsync(userId);
        return user == null
            ? NotFound()
            : Ok(new { user.Id, user.Email, user.Name });
    }
    //
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

}