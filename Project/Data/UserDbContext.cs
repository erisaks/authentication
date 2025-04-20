using Authentication.Entities;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Data;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
}