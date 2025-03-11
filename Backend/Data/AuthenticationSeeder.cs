using Backend.Models;
using Microsoft.AspNetCore.Identity;

namespace Backend.Data;

public class AuthenticationSeeder
{
    private RoleManager<IdentityRole> roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthenticationSeeder(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        this.roleManager = roleManager;
        _userManager = userManager;
    }
    
    public void AddRoles()
    {
        var tAdmin = CreateAdminRole(roleManager);
        tAdmin.Wait();

        var tUser = CreateUserRole(roleManager);
        tUser.Wait();
    }
    
    public void AddAdmin()
    {
        var tAdmin = CreateAdminIfNotExists();
        tAdmin.Wait();
    }

    private async Task CreateAdminIfNotExists()
    {
        var adminInDb = await _userManager.FindByEmailAsync("admin@admin.com");
        if (adminInDb == null)
        {
            var admin = new ApplicationUser{ UserName = "admin", Email = "admin@admin.com" };
            var adminCreated = await _userManager.CreateAsync(admin, "Admin0123");

            if (adminCreated.Succeeded)
            {
                await _userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }


    private async Task CreateAdminRole(RoleManager<IdentityRole> roleManager)
    {
        await roleManager.CreateAsync(new IdentityRole("Admin")); 
    }

    private async Task CreateUserRole(RoleManager<IdentityRole> roleManager)
    {
        await roleManager.CreateAsync(new IdentityRole("User"));
    }
}