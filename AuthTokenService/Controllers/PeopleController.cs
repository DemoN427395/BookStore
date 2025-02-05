using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthTokenService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PeopleController : ControllerBase
{

    [HttpGet]
    public IActionResult Get()
    {
        return Ok();
    }

    [HttpPost]
    public IActionResult Post()
    {
        return Ok();
    }
}