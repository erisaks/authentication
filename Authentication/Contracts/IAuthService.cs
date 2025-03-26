using Authentication.Entities;
using Authentication.Models.Dtos;

namespace Authentication.Contracts;

public interface IAuthService
{
    Task<User?> Register(UserDto request);
    Task<string?> Login(UserDto request);
}