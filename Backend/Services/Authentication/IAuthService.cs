using Backend.Controllers;
using Backend.DTOs;

namespace Backend.Services;

public interface IAuthService
{
    Task<RegistrationResult> RegisterAsync(string email, string password, string username);
    Task<LoginResult> LoginAsync(string email, string password);
}