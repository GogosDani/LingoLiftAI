using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class UsersContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    public UsersContext(DbContextOptions<UsersContext> options) : base(options)
    {
    }
}