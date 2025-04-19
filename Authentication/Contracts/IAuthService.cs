using Authentication.Models.Dtos;
using Authentication.Models.Dtos.Responses;

namespace Authentication.Contracts;

public interface IAuthService
{
    Task<RegisterUserResponseDto> Register(UserDto request);
    Task<TokenResponseDto?> Login(UserDto request);
    Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);

}