using Backend.Controllers;
using Backend.DTOs;
using Backend.Models;
using Microsoft.AspNetCore.Identity;

namespace Backend.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    public AuthService(UserManager<ApplicationUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }
    
    public async Task<RegistrationResult> RegisterAsync(string email, string password, string username)
    {
        var user = new ApplicationUser { UserName = username, Email = email };
        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            return new RegistrationResult(false,result.Errors.First().Description);
        }
        return new RegistrationResult(true, "");
    }

    public async Task<LoginResult> LoginAsync(string email, string password)
    {
        var managedUser = await _userManager.FindByEmailAsync(email);
        if (managedUser == null)
        {
            return new LoginResult(false, "Couldn't find user with this email", null);
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(managedUser, password);
        if (!isPasswordValid)
        {
            return new LoginResult(false, "Wrong password", null);
        }
        var accessToken = _tokenService.CreateToken(managedUser);
        return new LoginResult(true, "", accessToken);
    }
}