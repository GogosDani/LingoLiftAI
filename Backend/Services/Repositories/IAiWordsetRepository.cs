using Backend.Models;

namespace Backend.Services.Repositories;

public interface IAiWordSetRepository
{
    Task<AiWordSet> CreateWordSetAsync(AiWordSet wordSet, List<AiWordPair> wordPairs);
    Task<AiWordSet?> GetWordSetByIdAsync(int wordSetId);
    Task<List<AiWordSet>> GetWordSetsByUserIdAsync(string userId);
    Task DeleteWordSetAsync(int wordSetId);
}