using Backend.DTOs.UserDTOs;

namespace Backend.Services.Repositories;

public interface IUserRepository
{
    Task<UserInfoResponse> GetUserInfos(string id);
}