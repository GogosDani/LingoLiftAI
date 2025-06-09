using System.Text.Json;
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

    private string GenerateChallengePrompt(DateTime date)
    {
        var dayOfWeek = date.DayOfWeek;
        var isWeekend = dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday;
        var challengeTypes = new[] { "Vocabulary", "Translation", "GapFill" };
        var selectedType = challengeTypes[new Random().Next(challengeTypes.Length)];
        
        return $@"
Generate a Hungarian language learning challenge for {date:yyyy-MM-dd} ({dayOfWeek}).

Challenge type: {selectedType}
Difficulty level: {(isWeekend ? "Fun and creative" : "Standard practice")}

Please return the response EXACTLY in this JSON format:
{{
    ""content"": ""The complete challenge content including question, options, and correct answer"",
    ""type"": ""{selectedType}""
}}

Requirements for each type:

**Vocabulary**: Create a Hungarian vocabulary challenge with a word definition or synonym task.
Example content format:
""What does 'könyv' mean in English? A) Book B) Table C) Car D) House | Correct: A""

**Translation**: Create a translation challenge from English to Hungarian or vice versa.
Example content format:
""Translate to Hungarian: 'I love reading books.' A) Szeretek könyveket olvasni B) Szeretem a könyveket C) Könyveket szeretek D) Olvasom a könyveket | Correct: A""

**GapFill**: Create a sentence with missing words where users fill in the blanks.
Example content format:
""Fill in the blank: 'A macska ___ az asztalon.' A) ül B) fut C) úszik D) repül | Correct: A""

Additional requirements:
- Content should be in mixed Hungarian/English as appropriate for the challenge type
- Include 4 multiple choice options (A, B, C, D)
- Clearly indicate the correct answer
- Use this exact format: ""Question text A) Option1 B) Option2 C) Option3 D) Option4 | Correct: X""
- Make it engaging and educational
- {(isWeekend ? "Weekend challenges should be more creative and fun" : "Weekday challenges should focus on practical vocabulary")}
";
    }

    private DailyChallenge ParseAiResponseToChallenge(string aiResponse, DateTime date)
    {
        try
        {
            var jsonStart = aiResponse.IndexOf('{');
            var jsonEnd = aiResponse.LastIndexOf('}');
            if (jsonStart == -1 || jsonEnd == -1)
            {
                throw new InvalidOperationException("Invalid JSON format in AI response");
            }
            var jsonContent = aiResponse.Substring(jsonStart, jsonEnd - jsonStart + 1);
            var challengeData = JsonSerializer.Deserialize<ChallengeData>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (challengeData == null)
            {
                throw new InvalidOperationException("Failed to deserialize challenge data");
            }
            if (!Enum.TryParse<ChallengeType>(challengeData.Type, true, out var challengeType))
            {
                challengeType = ChallengeType.Vocabulary; // Default fallback
            }
            return new DailyChallenge
            {
                Date = date,
                Content = challengeData.Content,
                Type = challengeType
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing AI response to challenge");
            throw;
        }
    }

    private class ChallengeData
    {
        public string Content { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}