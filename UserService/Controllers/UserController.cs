using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    // Доступ разрешен только для аутентифицированных пользователей
    [HttpGet]
    // [Authorize]
    public IActionResult GetUserData()
    {
        // Доступ к информации о пользователе через HttpContext.User
        var userName = User.Identity?.Name;
        return Ok(new { Message = "User Data", User = userName });
    }

    [HttpGet("test")]
    public IActionResult TestUserData()
    {
        return Ok(new
        {
            Message = "Test"
        });
    }
}
