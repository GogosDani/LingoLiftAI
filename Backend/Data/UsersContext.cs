using Backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class UsersContext : IdentityUserContext<ApplicationUser>
{
    public UsersContext(DbContextOptions<UsersContext> options) : base(options)
    {
    }
}