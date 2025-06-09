using Backend.Models;

namespace Backend.Services.ChallengeServices;

public interface IDailyChallengeGeneratorService
{
    Task<DailyChallenge> GenerateChallengeForDateAsync(DateTime date);
    Task EnsureTodaysChallengeExistsAsync();
}