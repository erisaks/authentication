using Authentication.Contracts;
using Authentication.Models.Dtos;
using Authentication.Models.Enums;
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
        var response = await authService.Register(request);
        return response.ResponseStatus switch
        {
            ResponseStatus.Forbid => Forbid(),
            ResponseStatus.BadRequest => BadRequest(response),
            ResponseStatus.Ok => Ok(response),
            _ => BadRequest(response)
        };
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(UserDto request)
    {
        var response = await authService.Login(request);
        return response.ResponseStatus switch
        {
            ResponseStatus.Forbid => Forbid(),
            ResponseStatus.BadRequest => BadRequest(response),
            ResponseStatus.Ok => Ok(response),
            _ => BadRequest(response)
        };
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