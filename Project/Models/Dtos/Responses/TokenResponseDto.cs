using Authentication.Models.Dtos.Responses;
using Authentication.Models.Enums;

namespace Authentication.Models.Dtos.Responses;

public class TokenResponseDto(
    bool success,
    ResponseStatus responseStatus,
    string? accessToken = null,
    string? refreshToken = null,
    string[]? errors = null) : BaseResponseDto(success, responseStatus, errors)
{
    public string? AccessToken { get; set; } = accessToken;
    public string? RefreshToken { get; set; } = refreshToken;
}