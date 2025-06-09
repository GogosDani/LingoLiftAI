using Backend.DTOs.Challenge;
using Backend.Models;

namespace Backend.Services.Repositories;

public interface IChallengeRepository
{
    Task<DailyChallenge?> GetChallengeByDateAsync(DateTime date);
    Task<List<UserChallengeResultDto>> GetTop10UserResultsByDateAsync(DateTime date);
    Task<DailyChallenge> CreateChallengeAsync(DailyChallenge challenge);
}