using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Repositories;

public class LanguageRepository : ILanguageRepository
{
    
     private readonly LingoLiftContext _context;
    
        public LanguageRepository(LingoLiftContext context)
        {
            _context = context;
        }
    public async Task<IEnumerable<Language>> GetAllLanguages()
    {
        return await _context.Languages.ToListAsync();
    }
}