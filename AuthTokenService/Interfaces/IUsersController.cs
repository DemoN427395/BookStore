using Microsoft.AspNetCore.Mvc;

namespace AuthTokenService.Interfaces;

public interface IUsersController
{
    Task<IActionResult> GetCurrentUserId();
}