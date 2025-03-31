using Authentication.Entities;
using Authentication.Models.Dtos;

namespace Authentication.Contracts;

public interface IAuthService
{
    Task<User?> Register(UserDto request);
    Task<TokenResponseDto?> Login(UserDto request);
    Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
}