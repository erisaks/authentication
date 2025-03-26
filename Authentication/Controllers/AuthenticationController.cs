using Authentication.Contracts;
using Authentication.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthenticationController(IAuthService authService) : ControllerBase
{
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(UserDto request)
    {
        var user = await authService.Register(request);
        if (user is null)
        {
            return BadRequest("Something went wrong");
        }

        return Ok(user);
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(UserDto request)
    {
        var token = await authService.Login(request);
        if (string.IsNullOrEmpty(token)) return BadRequest();
        return Ok(token);
    }

    [Authorize]
    [HttpGet]
    public IActionResult AuthenticatedOnlyEndpoint()
    {
        return Ok("You are authenticated");
    }
}