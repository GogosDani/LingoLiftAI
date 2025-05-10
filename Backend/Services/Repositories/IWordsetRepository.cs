using Backend.Models;

namespace Backend.Services.Repositories;

public interface IWordsetRepository
{
    Task<int> CreateWordset(string userId, string setName, int firstLanguageId, int secondLanguageId);
    Task<WordPair> AddWordPair(int wordsetId);
    Task<bool> DeleteWordPair(int id);
    Task<bool> DeleteWordset(int id);
    Task<WordPair> EditWordPair(int id, string firstWord, string secondWord);
    Task<IEnumerable<CustomSet>> GetByUserId(string userId);
    Task<CustomSet> GetById(int id, string userId);
    Task<bool> EditWordset(int id, string name);
}