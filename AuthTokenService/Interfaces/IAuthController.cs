using BookStoreLib.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuthTokenService.Interfaces;

public interface IAuthController
{
    Task<IActionResult> Signup(SignupModel model);
    Task<IActionResult> Login(LoginModel model);
    Task<IActionResult> Revoke();
}