using System.Text.Json;
using Backend.DTOs.ChallengeDtos;
using Backend.Models;
using Backend.Services.AIServices;
using Backend.Services.Repositories;

namespace Backend.Services.ChallengeServices;

public class DailyChallengeGeneratorService : IDailyChallengeGeneratorService
{
    private readonly IChallengeRepository _challengeRepository;
    private readonly IAIClient _aiService; 
    private readonly ILogger<DailyChallengeGeneratorService> _logger;

    public DailyChallengeGeneratorService(
        IChallengeRepository challengeRepository,
        IAIClient aiService,
        ILogger<DailyChallengeGeneratorService> logger)
    {
        _challengeRepository = challengeRepository;
        _aiService = aiService;
        _logger = logger;
    }

    public async Task<DailyChallenge> GenerateChallengeForDateAsync(DateTime date)
    {
        try
        {
            var dateOnly = date.Date;
            var existingChallenge = await _challengeRepository.GetChallengeByDateAsync(dateOnly);
            if (existingChallenge != null)
            {
                return existingChallenge;
            }
            var prompt = GenerateChallengePrompt(dateOnly);
            var aiResponse = await _aiService.GetAiAnswer(prompt);
            _logger.LogWarning("AI response was: {Response}", aiResponse);
            var challenge = ParseAiResponseToChallenge(aiResponse, dateOnly);
            await _challengeRepository.CreateChallengeAsync(challenge);
            _logger.LogInformation($"Successfully generated challenge for {dateOnly:yyyy-MM-dd}");
            return challenge;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error generating challenge for {date:yyyy-MM-dd}");
            throw;
        }
    }

    public async Task EnsureTodaysChallengeExistsAsync()
    {
        var today = DateTime.Today;
        var challenge = await _challengeRepository.GetChallengeByDateAsync(today);
        
        if (challenge == null)
        {
            await GenerateChallengeForDateAsync(today);
        }
    }

    public async Task<UserAnswerEvaluationDto> EvaluateUserAnswersAsync(UserAnswerSubmissionDto userAnswers)
    {
        try
        {
            var today = DateTime.Today;
            var challenge = await _challengeRepository.GetChallengeByDateAsync(today);
            
            if (challenge == null)
            {
                throw new InvalidOperationException("No challenge found for today");
            }

            var evaluationPrompt = GenerateEvaluationPrompt(challenge, userAnswers);
            var aiResponse = await _aiService.GetAiAnswer(evaluationPrompt);
            var evaluation = ParseAiResponseToEvaluation(aiResponse);
            
            _logger.LogInformation($"Successfully evaluated user answers for challenge {challenge.Id}");
            return evaluation;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating user answers");
            throw;
        }
    }

    private string GenerateChallengePrompt(DateTime date)
    {
        var dayOfWeek = date.DayOfWeek;
        var isWeekend = dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday;
        var challengeTypes = new[] { "Vocabulary", "Translation"};
        var selectedType = challengeTypes[new Random().Next(challengeTypes.Length)];
        
        return $@"
Generate an English language learning challenge for {date:yyyy-MM-dd} ({dayOfWeek}).

Challenge type: {selectedType}

Please return the response EXACTLY in this JSON format:
{{
    ""content"": ""The complete challenge content"",
    ""type"": ""{selectedType}""
}}

Requirements for each type:

**Vocabulary**: Create exactly 10 English vocabulary words, nothing more just 10 english words, Only RESPONSE WITH THE 10 words nothing else
Example content format:
""1. book, 2. table, 3. chair, 4. mouse, ...""

**Translation**: Create exactly 10 sentences, nothing more just 10 sentences for translation practice,  Only RESPONSE WITH THE 10 Sentences nothing else.
Example content format:
""1. Translate: 'Szeretem a könyveket', 2. 'A macska az asztalon ül', ...""

Additional requirements:
- Do NOT include correct answers in the content
- The AI will evaluate user answers separately
- Focus on practical vocabulary and common phrases
";
    }

    private string GenerateEvaluationPrompt(DailyChallenge challenge, UserAnswerSubmissionDto userAnswers)
    {
        return $@"
Evaluate the user's answers for today's Hungarian language learning challenge.

Challenge Type: {challenge.Type}
Challenge Content: {challenge.Content}

User's Answers:
{string.Join(Environment.NewLine, userAnswers.Answers.Select((answer, index) => $"{index + 1}. {answer}"))}

Please evaluate each answer and return the response EXACTLY in this JSON format:
{{
    ""totalScore"": 0,
    ""maxScore"": 10,
}}
";
    }

    private UserAnswerEvaluationDto ParseAiResponseToEvaluation(string aiResponse)
    {
        try
        {
            var evaluation = JsonSerializer.Deserialize<UserAnswerEvaluationDto>(aiResponse);
            return evaluation;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse AI evaluation response");
            throw new InvalidOperationException("Failed to parse AI evaluation response", ex);
        }
    }

    private DailyChallenge ParseAiResponseToChallenge(string aiResponse, DateTime date)
    {
        try
        {
            var cleaned = CleanJson(aiResponse);
            var challengeData = JsonSerializer.Deserialize<ChallengeGenerationResponse>(cleaned);
            if (string.IsNullOrWhiteSpace(challengeData.Type) ||
                !Enum.TryParse<ChallengeType>(challengeData.Type, true, out var type))
            {
                throw new ArgumentException($"Invalid or missing challenge type: '{challengeData.Type}'");
            }
            return new DailyChallenge
            {
                Date = date,
                Content = challengeData.Content,
                Type = type,
            };
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse AI challenge response");
            throw new InvalidOperationException("Failed to parse AI challenge response", ex);
        }
    }
    
    private string CleanJson(string raw)
    {
        if (raw.TrimStart().StartsWith("```"))
        {
            var lines = raw.Split('\n');
            lines = lines.Where(line => !line.Trim().StartsWith("```")).ToArray();
            return string.Join("\n", lines).Trim();
        }
        return raw.Trim('`').Trim();
    }

}