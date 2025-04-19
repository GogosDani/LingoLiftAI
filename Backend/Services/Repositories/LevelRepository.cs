using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Repositories;

public class LevelRepository : ILevelRepository
{
    
    private readonly LingoLiftContext _context;
    
    public LevelRepository(LingoLiftContext context)
    {
        _context = context;
    }
    
   public async Task<Level> GetUserLevelByLanguageId(int languageId, string userId)
   {
       var entity = await _context.UserLanguageLevels
           .Include(x => x.Level)
           .FirstOrDefaultAsync(x => x.LanguageId == languageId && x.UserId == userId);
       return entity.Level;
   }
}