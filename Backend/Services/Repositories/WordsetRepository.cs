using Backend.Data;
using Backend.DTOs.WordsetDTOs;
using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Repositories;

public class WordsetRepository : IWordsetRepository
{
    
    private readonly LingoLiftContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    
    public WordsetRepository(LingoLiftContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    public async Task<int> CreateWordset(string userId, string setName, int firstLanguageId, int secondLanguageId)
    {
        var userExists = await _userManager.FindByIdAsync(userId);
        if (userExists == null)
        {
            throw new Exception($"User with ID {userId} does not exist.");
        }
        if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException("User id cannnot be null");
        if (string.IsNullOrEmpty(setName)) throw new ArgumentNullException("Set name cannnot be null");
        var set = new CustomSet{FirstLanguageId = firstLanguageId, SecondLanguageId = secondLanguageId, UserId = userId, Name = setName, WordPairs = new List<WordPair>()};
        _context.Sets.Add(set);
        await _context.SaveChangesAsync();
        return set.Id;
    }

    public async Task<WordPair> AddWordPair(int wordsetId)
    {
        var set = await _context.Sets.FindAsync(wordsetId);
        if (set == null) throw new KeyNotFoundException($"Wordset with ID {wordsetId} not found");
        var wordpair = new WordPair { FirstWord = "", SecondWord = "", SetId = wordsetId };
        _context.WordPairs.Add(wordpair);
        await _context.SaveChangesAsync();
        return wordpair;
    }

    public async Task<bool> DeleteWordPair(int id)
    {
        var wordpair = await _context.WordPairs.FindAsync(id);
        if (wordpair == null) return false;
        _context.WordPairs.Remove(wordpair);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteWordset(int id)
    {
        var wordset = await _context.Sets.FirstOrDefaultAsync(ws => ws.Id == id);
        if (wordset == null) return false;
        _context.Sets.Remove(wordset);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<WordPair> EditWordPair(int id, string firstWord, string secondWord)
    {
        var wordpair = await _context.WordPairs.FindAsync(id);
        if (wordpair == null)
            throw new KeyNotFoundException($"WordPair with ID {id} not found");
        wordpair.FirstWord = firstWord;
        wordpair.SecondWord = secondWord;
        await _context.SaveChangesAsync();
        return wordpair;
    }
    public async Task<IEnumerable<WordsetResponse>> GetByUserId(string userId)
    {
        var sets = await _context.Sets
            .Include(s => s.WordPairs)
            .Where(s => s.UserId == userId)
            .Select(s => new 
            {
                Set = s,
                FirstLanguage = _context.Languages.FirstOrDefault(l => l.Id == s.FirstLanguageId),
                SecondLanguage = _context.Languages.FirstOrDefault(l => l.Id == s.SecondLanguageId)
            })
            .ToListAsync();
        return sets.Select(item => 
            new WordsetResponse(
                item.Set.Id, 
                item.Set.Name,
                item.FirstLanguage,
                item.SecondLanguage,
                item.Set.WordPairs.Select(wp => 
                    new WordPairResponse(wp.Id, wp.FirstWord, wp.SecondWord)
                ).ToList()
            )
        ).ToList();
    }

    public async Task<CustomSet> GetById(int id, string userId)
    {
        var set = await _context.Sets
            .Include(s => s.WordPairs)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (set == null) throw new InvalidOperationException("Set not found!");
        if (set.UserId != userId) throw new InvalidOperationException("This set is not owned by this user!");
        return set;
    }
    
    public async Task<bool> EditWordset(int id, string name)
    {
        var set = await _context.Sets.FindAsync(id);
        if(set == null) return false;
        set.Name = name;
        await _context.SaveChangesAsync();
        var updatedSet = await _context.Sets.FindAsync(id);
        return true;
    }
}