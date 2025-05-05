using Backend.Data;
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
}