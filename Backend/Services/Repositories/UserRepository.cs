using Backend.DTOs.UserDTOs;
using Backend.Models;
using Microsoft.AspNetCore.Identity;

namespace Backend.Services.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserRepository(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserInfoResponse> GetUserInfos(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return null;
        return new UserInfoResponse(user.Email, user.UserName, userId);
    }
    public async Task<string> GetPasswordResetToken(string email)
    {
        ApplicationUser? user = await _userManager.FindByEmailAsync(email);
        if(user == null) throw new InvalidOperationException("Couldn't find user with this email");
        var token =  await _userManager.GeneratePasswordResetTokenAsync(user);
        return Uri.EscapeDataString(token);
    }

    public async Task<bool> ResetPassword(string email, string token, string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            throw new InvalidOperationException("Couldn't find user with this email");
        }
         var decodedToken = Uri.UnescapeDataString(token);
        var result = await _userManager.ResetPasswordAsync(user, decodedToken, newPassword);
        return result.Succeeded;
    }
    public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }
        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        return result.Succeeded;
    }

}