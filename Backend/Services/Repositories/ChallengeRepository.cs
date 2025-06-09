 using Backend.Data;
 using Backend.DTOs.Challenge;
 using Backend.Models;
 using Backend.Services.Repositories;
 using Microsoft.EntityFrameworkCore;

 public class ChallengeRepository : IChallengeRepository
    {
        private readonly LingoLiftContext _context;

        public ChallengeRepository(LingoLiftContext context)
        {
            _context = context;
        }
        public async Task<DailyChallenge?> GetChallengeByDateAsync(DateTime date)
        {
            var dateOnly = date.Date;
            return await _context.DailyChallenges
                .FirstOrDefaultAsync(dc => EF.Functions.DateDiffDay(dc.Date, dateOnly) == 0);
        }
        public async Task<List<UserChallengeResultDto>> GetTop10UserResultsByDateAsync(DateTime date)
        {
            var dateOnly = date.Date;
            var challenge = await _context.DailyChallenges
                .FirstOrDefaultAsync(dc => EF.Functions.DateDiffDay(dc.Date, dateOnly) == 0);
            if (challenge == null)
            {
                return new List<UserChallengeResultDto>();
            }
            var results = await _context.UserChallenges
                .Where(uc => uc.DailyChallengeId == challenge.Id)
                .Include(uc => uc.User)
                .OrderByDescending(uc => uc.Score)
                .ThenBy(uc => uc.CompletedAt) 
                .Take(10)
                .Select(uc => new UserChallengeResultDto
                {
                    UserId = uc.UserId,
                    Username = uc.User!.UserName ?? "Unknown",
                    Score = uc.Score,
                    CompletedAt = uc.CompletedAt,
                })
                .ToListAsync();
            return results;
        }
        public async Task<DailyChallenge> CreateChallengeAsync(DailyChallenge challenge)
        {
            _context.DailyChallenges.Add(challenge);
            await _context.SaveChangesAsync();
            return challenge;
        }
    }