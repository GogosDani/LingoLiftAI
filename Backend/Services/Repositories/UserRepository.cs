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
        return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<bool> ResetPassword(string email, string token, string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            throw new InvalidOperationException("Couldn't find user with this email");
        }
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        return result.Succeeded;
    }
}