using Authentication.Models.Enums;

namespace Authentication.Models.Dtos.Responses;

public class BaseResponseDto
{
    public bool Success { get; set; }
    public ResponseStatus ResponseStatus { get; set; }
    public string ResponseType => ResponseStatus.ToString();
    public string[]? Errors { get; set; }

    protected BaseResponseDto() { }

    protected BaseResponseDto(
        bool success,
        ResponseStatus responseStatus,
        string[]? errors = null)
    {
        Success = success;
        ResponseStatus = responseStatus;
        Errors = errors;
    }

    public BaseResponseDto Ok()
    {
        return new BaseResponseDto(success: true, responseStatus: ResponseStatus.Ok);
    }
    
    public BaseResponseDto BadRequest(string[] errors)
    {
        return new BaseResponseDto(
            success: false,
            responseStatus: ResponseStatus.BadRequest,
            errors: errors);
    }

    public BaseResponseDto Forbid()
    {
        return new BaseResponseDto(success: false, responseStatus: ResponseStatus.Forbid);
    }
}