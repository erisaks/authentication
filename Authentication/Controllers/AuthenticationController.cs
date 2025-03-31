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
        var response = await authService.Login(request);
        return Ok(response);
    }

    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto request)
    {
        var response = await authService.RefreshTokensAsync(request);
        if (response is null) return Unauthorized("Invalid refresh token");
        return Ok(response);
    }

    [Authorize]
    [HttpGet]
    public IActionResult AuthenticatedOnlyEndpoint()
    {
        return Ok("You are authenticated");
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin-only")]
    public IActionResult AdminOnlyEndpoint()
    {
        return Ok("You are an administrator");
    }
}