using Backend.Models;

namespace Backend.Services.Repositories;

public interface ILevelRepository
{
    Task<Level> GetUserLevelByLanguageId(int languageId, string userId);
}