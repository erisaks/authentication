using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Authentication.Contracts;
using Authentication.Data;
using Authentication.Entities;
using Authentication.Models.Dtos;
using Authentication.Models.Dtos.Responses;
using Authentication.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Authentication.Services;

public class AuthService(
    UserDbContext userDb, 
    IConfiguration configuration) : IAuthService
{
    #region Public Methods
    public async Task<RegisterUserResponseDto> Register(UserDto request)
    {
        if (await ValidateIfUserExistsAsync(request.Email))
        {
             return new RegisterUserResponseDto(
                success: false,
                responseStatus: ResponseStatus.BadRequest,
                errors: ["Failed to register user"]);
        }
        
        var user = User.New(email: request.Email, password: request.Password);

        userDb.Users.Add(user);
        var id = await userDb.SaveChangesAsync();
        if (id == 0)
        {
            return new RegisterUserResponseDto(
                success: false,
                responseStatus: ResponseStatus.BadRequest,
                errors: ["Failed to register user"]);
        }

        return new RegisterUserResponseDto(
            success: true,
            responseStatus: ResponseStatus.Ok,
            id: user.Id.ToString(),
            email: user.Email);
    }

    public async Task<TokenResponseDto> Login(UserDto request)
    {
        var user = await userDb.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
        if (user is null)
        {
            return new TokenResponseDto(
                success: false,
                responseStatus: ResponseStatus.BadRequest,
                errors: ["Invalid credentials"]);
        }

        if (!VerifyPassword(user, request.Password))
        {
            return new TokenResponseDto(success: false, responseStatus: ResponseStatus.Forbid);
        }
        
        return await CreateTokens(user);
    }

    public async Task<TokenResponseDto> RefreshTokensAsync(RefreshTokenRequestDto request)
    {
        var user = await ValidateRefreshTokenAsync(request);
        return await CreateTokens(user);
    }
    #endregion
    
    #region Private Methods

    private async Task<bool> ValidateIfUserExistsAsync(string email)
    {
        return await userDb.Users.AnyAsync(u => u.Email == email);
    }

    private static bool VerifyPassword(User user, string password)
    {
        var passwordHasher = new PasswordHasher<User>();
        var verifyPasswordResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return verifyPasswordResult == PasswordVerificationResult.Success;
    }
    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(
            key: Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));
        
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var token = new JwtSecurityToken(
            issuer: configuration.GetValue<string>("AppSettings:Issuer"),
            audience: configuration.GetValue<string>("AppSettings:Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: credentials);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
    
    private async Task<TokenResponseDto> CreateTokens(User? user)
    {
        if (user is null)
        {
            return new TokenResponseDto(success: false, responseStatus: ResponseStatus.NotFound);
        }
        var accessToken = CreateToken(user);
        var refreshToken = await GenerateAndSaveRefreshTokenAsync(user);

        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
        {
            return new TokenResponseDto(
                success: false,
                responseStatus: ResponseStatus.BadRequest,
                errors: ["Failed to generate tokens"]);
        }
        
        return new TokenResponseDto(
            success: true,
            responseStatus: ResponseStatus.Ok,
            accessToken: accessToken,
            refreshToken: refreshToken);
    }

    private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
    {
        var refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(1);
        await userDb.SaveChangesAsync();
        return refreshToken;
    }

    private async Task<User?> ValidateRefreshTokenAsync(RefreshTokenRequestDto request)
    {
        var user = await userDb.Users.FindAsync(request.UserId);
        if (user is null 
            || user.RefreshToken != request.RefreshToken 
            || user.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            return null;
        }

        return user;
    }
    #endregion
}