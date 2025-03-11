using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services;

public class TokenService : ITokenService
{
    private const int ExpMinutes = 30;
    private readonly IConfiguration _configuration;
    
    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public string CreateToken(ApplicationUser user, string role)
    {
        var expiration = DateTime.UtcNow.AddMinutes(ExpMinutes);
        var claims = CreateClaims(user, role);
        var credentials = CreateCredentials();
        
        var token = CreateJwtToken(
            claims,
            credentials,
            expiration
        );
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }
    
    private JwtSecurityToken CreateJwtToken(List<Claim> claims, SigningCredentials credentials,
        DateTime expiration) =>
        new(
            _configuration["ValidIssuer"],
            _configuration["ValidAudience"],
            claims,
            expires: expiration,
            signingCredentials: credentials
        );
    
    private List<Claim> CreateClaims(ApplicationUser user, string? role)
    {
        try
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(DateTime.UtcNow).ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };
            if (role != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private SigningCredentials CreateCredentials()
    {
        return new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JwtSecretKey"])
            ),
            SecurityAlgorithms.HmacSha256
        );
    }
}