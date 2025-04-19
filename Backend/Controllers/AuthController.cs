using Microsoft.AspNetCore.Mvc;
using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationRequest request)
    {
        try
        {
            var result = await _authService.RegisterAsync(request.Email, request.Password, request.Username, "User");
            if (result.errorMessage != "") return BadRequest(result.errorMessage);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPost("login")]
    public async Task<ActionResult> Authenticate([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request.Email, request.Password);
        Response.Cookies.Append("jwt", result.Token, new CookieOptions
        {
            HttpOnly = true,  
            SameSite = SameSiteMode.Strict, 
            Expires = DateTime.UtcNow.AddHours(0.5) 
        });
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }
        return Ok(result.Token);
    }

    [HttpGet("check")]
    public IActionResult CheckAuthentication()
    {
        if (Request.Cookies.ContainsKey("jwt")) return Ok("Authenticated!");
        return Unauthorized("Not Authenticated!");
    }
    
    [HttpPost("logout"), Authorize]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt", new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict
        });
        return Ok(new { message = "Logged out successfully" });
    }
}