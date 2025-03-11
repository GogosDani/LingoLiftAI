using Backend.Models;

namespace Backend.Services;

public interface ITokenService
{
    public string CreateToken(ApplicationUser user);
}