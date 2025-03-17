using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Repositories;

public class UserLanguageRepository : IUserLanguageRepository
{
    private readonly LingoLiftContext _context;

    public UserLanguageRepository(LingoLiftContext context)
    {
        _context = context;
    }
    public async Task<bool> UserHasLanguageLevel(string userId)
    {
        return await _context.UserLanguageLevels.AnyAsync(l => l.UserId == userId);
    }

    public async Task AddUserLanguageLevel(string userId, Languages language, string level)
    {
        var levelEntity = await _context.Levels.FirstOrDefaultAsync(l => l.LevelName == level.Trim());
        if (levelEntity == null)
            throw new Exception($"Level '{level}' not found.");
        var levelId = levelEntity.Id;
        
        var languageEntity = await _context.Languages.FirstOrDefaultAsync(l => l.LanguageName == language.ToString());
        if (languageEntity == null)
            throw new Exception($"Language '{language}' not found.");

        var languageId = languageEntity.Id;
        var userLanguage = new UserLanguage
        {
            UserId = userId,
            LanguageId = languageId,
            LevelId = levelId
        };
        _context.UserLanguageLevels.Add(userLanguage);
        await _context.SaveChangesAsync();
    }

}