using Backend.Models;

namespace Backend.Services.Repositories;

public interface IUserLanguageRepository
{
    public Task<bool> UserHasLanguageLevel(string userId, int languageId);
    public Task<bool> UserHasLevel(string userId);
    public Task AddUserLanguageLevel(string userId, int languageId, string level);
    public Task ChangeLanguageLevel(string userId, int languageId, string level);
    public Task<string> GetUserLanguageLevel(string userId, int languageId);
    public Task<string> GetPreviousLevel(string level);
    public Task<string> GetNextLevel(string level);
    public Task<Language> GetLanguageById(int id);
}