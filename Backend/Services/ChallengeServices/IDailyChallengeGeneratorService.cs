using Backend.DTOs.ChallengeDtos;
using Backend.Models;

namespace Backend.Services.ChallengeServices;

public interface IDailyChallengeGeneratorService
{
    Task<DailyChallenge> GenerateChallengeForDateAsync(DateTime date);
    Task EnsureTodaysChallengeExistsAsync();
    Task<UserAnswerEvaluationDto> EvaluateUserAnswersAsync(UserAnswerSubmissionDto userAnswers);
}