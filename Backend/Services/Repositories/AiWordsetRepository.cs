using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Repositories;


    public class AiWordsetRepository : IAiWordSetRepository
    {
        private readonly LingoLiftContext _context;
    
        public AiWordsetRepository(LingoLiftContext context)
        {
            _context = context;
        }
        public async Task<AiWordSet> CreateWordSetAsync(AiWordSet wordSet, List<AiWordPair> wordPairs)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.AiWordSets.Add(wordSet);
                await _context.SaveChangesAsync();

                foreach (var wordPair in wordPairs)
                {
                    wordPair.WordSetId = wordSet.Id;
                    _context.AiWordPairs.Add(wordPair);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return wordSet;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<AiWordSet?> GetWordSetByIdAsync(int wordSetId)
        {
            return await _context.AiWordSets
                .Include(ws => ws.Topic)
                .Include(ws => ws.WordPairs)
                .FirstOrDefaultAsync(ws => ws.Id == wordSetId);
        }

        public async Task<List<AiWordSet>> GetWordSetsByUserIdAsync(string userId)
        {
            return await _context.AiWordSets
                .Include(ws => ws.Topic)
                .Include(ws => ws.WordPairs)
                .Where(ws => ws.UserId == userId)
                .OrderByDescending(ws => ws.CreatedAt)
                .ToListAsync();
        }

        public async Task DeleteWordSetAsync(int wordSetId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var wordPairs = await _context.AiWordPairs
                    .Where(w => w.WordSetId == wordSetId)
                    .ToListAsync();

                _context.AiWordPairs.RemoveRange(wordPairs);

                var wordSet = await _context.AiWordSets.FindAsync(wordSetId);
                if (wordSet != null)
                {
                    _context.AiWordSets.Remove(wordSet);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
}
