using Authentication.Models.Enums;

namespace Authentication.Models.Dtos.Responses;
public class LoginUserResponseDto(
    bool success,
    ResponseStatus responseStatus,
    string[]? errors = null,
    string? token = null) : BaseResponseDto(success, responseStatus, errors)
{
    public string? Token { get; set; } = token;
}