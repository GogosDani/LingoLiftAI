using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;
[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet, Authorize]
    public async Task<IActionResult> GetUserInfos()
    {
        try
        {
            if (!HttpContext.Request.Cookies.TryGetValue("jwt", out var token))
            {
                return null;
            }
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var userData = await _userRepository.GetUserInfos(userId);
            return Ok(userData);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
}