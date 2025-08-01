using Backend.DTOs.UserDTOs;

namespace Backend.Services.Repositories;

public interface IUserRepository
{
    Task<UserInfoResponse> GetUserInfos(string id);
    public Task<string> GetPasswordResetToken(string email);
    public Task<bool> ResetPassword(string email, string token, string newPassword);
    public Task<bool> ChangePasswordAsync(string currentPassword, string newPassword, string userId);
}