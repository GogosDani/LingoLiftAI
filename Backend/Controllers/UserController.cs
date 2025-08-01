using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend.DTOs;
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
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPwdRequest request)
    {
        try
        {
            if (request == null)
            {
                return BadRequest("Request cannot be null");
            }
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest("Email cannot be empty");
            }
            if (!IsValidEmail(request.Email))
            {
                return BadRequest("Invalid email format");
            }
            var token = await _userRepository.GetPasswordResetToken(request.Email);
            string resetLink = $"{_configuration["FrontendUrl"]}/reset/password/new?email={Uri.EscapeDataString(request.Email)}&token={token}";
            await _emailSender.SendEmailAsync(
                request.Email,
                "Password Reset",
                $"Please reset your password by clicking <a href='{resetLink}'>here</a>");
            
            return Ok("Password reset link sent to your email.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPwdRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.NewPassword))
            {
                return BadRequest("Invalid request.");
            }
            var succeed = await _userRepository.ResetPassword(request.Email, request.Token, request.NewPassword);
            if(!succeed) return BadRequest("Error occured while resetting password.");
            return Ok("Password reset successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPatch]
    public async Task<IActionResult> ChangeUserPassword([FromBody] ChangePasswordRequest model)
    {
        var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("UserId not found.");
        }
        var success = await _userRepository.ChangePasswordAsync(model.CurrentPassword, model.NewPassword, userId);
        if (!success)
        {
            return BadRequest("Failed to change password.");
        }
        return Ok("Password changed successfully.");
    }
    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
    
}