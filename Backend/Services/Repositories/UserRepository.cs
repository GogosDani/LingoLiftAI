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
}