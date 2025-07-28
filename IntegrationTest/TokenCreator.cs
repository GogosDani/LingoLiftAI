using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Identity;

public class TokenCreator
{
    public ITokenService TokenService;
    public TokenCreator(ITokenService tokenService)
    {
        TokenService = tokenService;
    }
    public string GenerateJwtToken(string role = "User")
    {
        var user = new ApplicationUser{UserName = "TestUser", Email = "TestEmail"};
        return TokenService.CreateToken(user, role);
    }
}