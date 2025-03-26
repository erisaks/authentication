using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Authentication.Contracts;
using Authentication.Data;
using Authentication.Entities;
using Authentication.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Authentication.Services;

public class AuthService(
    UserDbContext userDb, 
    IConfiguration configuration) : IAuthService
{
    #region Public Methods
    public async Task<User?> Register(UserDto request)
    {
        if (await ValidateIfUserExistsAsync(request.Email))
        {
            return null;
        }
        
        var user = User.New(email: request.Email, password: request.Password);

        userDb.Users.Add(user);
        await userDb.SaveChangesAsync();

        return user;
    }

    public async Task<string?> Login(UserDto request)
    {
        var user = await userDb.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
        if (user is null) return null;

        if (!VerifyPassword(user, request.Password)) return null;

        var token = CreateToken(user);
        return token;
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
            new(ClaimTypes.NameIdentifier, user.Id.ToString())
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
    #endregion
}