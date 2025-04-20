using Authentication.Models.Enums;

namespace Authentication.Models.Dtos.Responses;
public class RegisterUserResponseDto(
    bool success,
    ResponseStatus responseStatus,
    string[]? errors = null,
    string? id = "",
    string? email = "") : BaseResponseDto(success, responseStatus, errors)
{
    public string? Id { get; set; } = id;
    public string? Email { get; set; } = email;
}