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

   
    public async Task<Language> GetById(int id)
    {
        var language = await _context.Languages.FindAsync(id);
        if (language == null)
        {
            throw new InvalidOperationException("Couldn't find language with this id");
        }
        return language;
    }
}