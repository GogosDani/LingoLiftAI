using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;
[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;

    public UserController(IUserRepository userRepository,  IEmailSender emailSender, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _emailSender = emailSender;
        _configuration = configuration;
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
    
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest("Email cannot be empty");
            }

            var token = await _userRepository.GetPasswordResetToken(request.Email);
            string resetLink = $"{_configuration["FrontendUrl"]}/ResetPassword?token={token}";
            await _emailSender.SendEmailAsync(
                request.Email,
                "Password Reset",
                $"Please reset your password by clicking <a href='{resetLink}'>here</a>.");
            return Ok("Password reset link sent to your email.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
}