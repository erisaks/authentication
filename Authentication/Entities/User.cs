using Microsoft.AspNetCore.Identity;

namespace Authentication.Entities;

public class User
{
    #region Entity Properties
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    #endregion

    private User(
        Guid id,
        string email,
        string passwordHash)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
    }
    #region Public Entity Methods

    public static User New(string email, string password)
    {
        var user = new User(
            id: Guid.Empty,
            email: email,
            passwordHash: string.Empty);

        user.SetPasswordHash(password);
        
        return user;
    }
    #endregion
    
    #region Private Entity Methods

    private void SetPasswordHash(string password)
    {
        PasswordHash = CreatePasswordHash(password);
    }

    private string CreatePasswordHash(string password)
    {
        return new PasswordHasher<User>().HashPassword(this, password);
    }
    #endregion
}