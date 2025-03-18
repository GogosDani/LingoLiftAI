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

    public async Task<bool> UserHasLevel(string userId)
    {
        return await _context.UserLanguageLevels.AnyAsync(l => l.UserId == userId);
    }
    public async Task<bool> UserHasLanguageLevel(string userId, int languageId)
    {
        return await _context.UserLanguageLevels.AnyAsync(l => l.UserId == userId && l.LanguageId == languageId);
    }

    public async Task<string> GetUserLanguageLevel(string userId, int languageId)
    {
        var entity = await _context.UserLanguageLevels
            .Include(l => l.Level) 
            .FirstOrDefaultAsync(l => l.UserId == userId && l.LanguageId == languageId);

        if (entity != null && entity.Level != null)
        {
            return entity.Level.LevelName;
        }
        throw new Exception("Couldn't find the level!");
    }

    public async Task<string> GetPreviousLevel(string level)
    {
        var currentLevel = await _context.Levels.FirstOrDefaultAsync(l => l.LevelName == level);
        if (currentLevel == null || currentLevel.Id == 1) return null;
        var previousLevel = await _context.Levels.FirstOrDefaultAsync(l => l.Id == currentLevel.Id - 1);
        return previousLevel?.LevelName;
    }

    public async Task<string> GetNextLevel(string level)
    {
        var currentLevel = await _context.Levels.FirstOrDefaultAsync(l => l.LevelName == level);
        if (currentLevel == null) return null; 
        var nextLevel = await _context.Levels.FirstOrDefaultAsync(l => l.Id == currentLevel.Id + 1);
        return nextLevel?.LevelName;
    }

    public async Task<Language> GetLanguageById(int id)
    {
       return await _context.Languages.FirstOrDefaultAsync(l => l.Id == id);
    }


    // After writing test, user gets a language level
    public async Task AddUserLanguageLevel(string userId, int languageId, string level)
    {
        var levelId = await GetLevelId(level);
        var userLanguage = new UserLanguage
        {
            UserId = userId,
            LanguageId = languageId,
            LevelId = levelId
        };
        _context.UserLanguageLevels.Add(userLanguage);
        await _context.SaveChangesAsync();
    }

    
    public async Task ChangeLanguageLevel(string userId, int languageId, string level)
    {
        var levelId = await GetLevelId(level);
        var entity = await _context.UserLanguageLevels
            .FirstOrDefaultAsync(l => l.UserId == userId && l.LanguageId == languageId);
        if (entity != null)
        {
            entity.LevelId = levelId; 
            await _context.SaveChangesAsync(); 
        }
        else
        {
            throw new Exception("User language level not found.");
        }
    }

    


    private async Task<int> GetLevelId(string level)
    {
        var levelEntity = await _context.Levels.FirstOrDefaultAsync(l => l.LevelName == level.Trim());
        if (levelEntity == null) throw new Exception($"Level '{level}' not found.");
        return levelEntity.Id;
    }

}