using Backend.Models;

namespace Backend.Services.Repositories;

public interface IUserLanguageRepository
{
    public Task<bool> UserHasLanguageLevel(string userId);
    public Task AddUserLanguageLevel(string userId, Languages language, string level);
}