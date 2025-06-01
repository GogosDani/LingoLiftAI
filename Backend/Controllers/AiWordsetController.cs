using System.Security.Claims;
using System.Text.Json;
using Backend.DTOs.WordsetDTOs;
using Backend.Exceptions.AIExceptions;
using Backend.Models;
using Backend.Services.AIServices;
using Backend.Services.Repositories;
using Microsoft.AspNetCore.Mvc;

[Route("api/ai-wordset")]
[ApiController]
public class AiWordsetController : ControllerBase
{
    private readonly IAiWordSetRepository _wordSetRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IAIClient _aiClient;

    private readonly string[] difficultyLevels =
        { "beginner", "elementary", "intermediate", "upper intermediate", "advanced", "proficient" };

    public AiWordsetController(IAiWordSetRepository wordSetRepository, ITopicRepository topicRepository,
        IAIClient aiClient)
    {
        _wordSetRepository = wordSetRepository;
        _topicRepository = topicRepository;
        _aiClient = aiClient;
    }

    [HttpGet("topics")]
    public async Task<ActionResult> GetAvailableTopics()
    {
        try
        {
            var topics = await _topicRepository.GetAllTopicsAsync();
            return Ok(topics.Select(t => new { t.Id, t.Name, t.Description }));
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync(ex.Message);
            return StatusCode(500, new { message = "An unexpected error occurred while fetching topics." });
        }
    }

    [HttpGet("popular-topics")]
    public async Task<ActionResult> GetPopularTopics([FromQuery] int count = 10)
    {
        try
        {
            var topics = await _topicRepository.GetTopPopularTopicsAsync(count);
            return Ok(topics.Select(t => new { t.Id, t.Name, t.Description, t.Popularity }));
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync(ex.Message);
            return StatusCode(500, new { message = "An unexpected error occurred while fetching popular topics." });
        }
    }

    [HttpPost("generate")]
    public async Task<ActionResult> GenerateWordset(GenerateWordsetRequest request)
    {
        try
        {
            // Validáció
            if (request.WordCount <= 0 || request.WordCount > 50)
            {
                return BadRequest(new { message = "Word count must be between 1 and 50." });
            }

            if (!difficultyLevels.Contains(request.DifficultyLevel.ToLower().Trim()))
            {
                return BadRequest(new { message = "Invalid difficulty level provided." });
            }

            var topic = await _topicRepository.GetTopicByIdAsync(request.TopicId);
            if (topic == null)
            {
                return NotFound(new { message = "Topic not found." });
            }

            // AI prompt
            var aiPrompt =
                $"Generate exactly {request.WordCount} words for {request.DifficultyLevel} level learners about the topic: {topic.Name} - {topic.Description}. " +
                $"Each word should be appropriate for {request.DifficultyLevel} level. " +
                "Format your response as JSON array with objects containing 'word' and 'translation' fields. " +
                "Example: [{\"word\":\"example\",\"translation\":\"példa\"}, {\"word\":\"test\",\"translation\":\"teszt\"}]. " +
                "Do not include any additional text, only the JSON array.";

            var aiResponse = await _aiClient.GetAiAnswer(aiPrompt);
            var wordPairs = ParseAiResponse(aiResponse);

            if (wordPairs == null || wordPairs.Count != request.WordCount)
            {
                return StatusCode(500, new { message = $"AI did not return exactly {request.WordCount} word pairs." });
            }

            // WordSet létrehozása
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not authenticated." });
            }

            var wordSet = new AiWordSet
            {
                Name = $"{topic.Name} - {request.DifficultyLevel}",
                Description = $"AI generated {request.DifficultyLevel} level words about {topic.Name}",
                TopicId = request.TopicId,
                UserId = userId,
                DifficultyLevel = request.DifficultyLevel,
                CreatedAt = DateTime.UtcNow
            };

            var createdWordSet = await _wordSetRepository.CreateWordSetAsync(wordSet, wordPairs);

            await _topicRepository.IncrementPopularityAsync(request.TopicId);

            return Ok(new
            {
                wordSetId = createdWordSet.Id,
                name = createdWordSet.Name,
                wordCount = wordPairs.Count,
                words = wordPairs.Select(wp => new { word = wp.FirstWord, translation = wp.SecondWord })
            });
        }
        catch (NullGeminiUrlException ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
        catch (NullAnswerException ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync(ex.Message);
            return StatusCode(500, new { message = "An unexpected error occurred while generating wordset." });
        }
    }

    [HttpGet("my-wordsets")]
    public async Task<ActionResult> GetMyWordsets()
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not authenticated." });
            }

            var wordSets = await _wordSetRepository.GetWordSetsByUserIdAsync(userId);
            
            return Ok(wordSets.Select(ws => new
            {
                id = ws.Id,
                name = ws.Name,
                description = ws.Description,
                difficultyLevel = ws.DifficultyLevel,
                wordCount = ws.WordPairs.Count,
                topicName = ws.Topic.Name,
                createdAt = ws.CreatedAt
            }));
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync(ex.Message);
            return StatusCode(500, new { message = "An unexpected error occurred while fetching wordsets." });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetWordset(int id)
    {
        try
        {
            var wordSet = await _wordSetRepository.GetWordSetByIdAsync(id);
            if (wordSet == null)
            {
                return NotFound(new { message = "Wordset not found." });
            }

            var userId = GetUserId();
            if (wordSet.UserId != userId)
            {
                return Forbid();
            }

            return Ok(new
            {
                id = wordSet.Id,
                name = wordSet.Name,
                description = wordSet.Description,
                difficultyLevel = wordSet.DifficultyLevel,
                topicName = wordSet.Topic.Name,
                createdAt = wordSet.CreatedAt,
                words = wordSet.WordPairs.Select(wp => new 
                { 
                    id = wp.Id,
                    word = wp.FirstWord, 
                    translation = wp.SecondWord 
                })
            });
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync(ex.Message);
            return StatusCode(500, new { message = "An unexpected error occurred while fetching wordset." });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteWordset(int id)
    {
        try
        {
            var wordSet = await _wordSetRepository.GetWordSetByIdAsync(id);
            if (wordSet == null)
            {
                return NotFound(new { message = "Wordset not found." });
            }

            var userId = GetUserId();
            if (wordSet.UserId != userId)
            {
                return Forbid();
            }

            await _wordSetRepository.DeleteWordSetAsync(id);
            return Ok(new { message = "Wordset deleted successfully." });
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync(ex.Message);
            return StatusCode(500, new { message = "An unexpected error occurred while deleting wordset." });
        }
    }

    private List<AiWordPair>? ParseAiResponse(string aiResponse)
    {
        try
        {
            var cleanResponse = aiResponse.Trim();
            if (cleanResponse.StartsWith("```json"))
            {
                cleanResponse = cleanResponse.Substring(7);
            }
            if (cleanResponse.EndsWith("```"))
            {
                cleanResponse = cleanResponse.Substring(0, cleanResponse.Length - 3);
            }

            var wordPairDtos = JsonSerializer.Deserialize<List<AiWordpairDTO>>(cleanResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return wordPairDtos?.Select(dto => new AiWordPair
            {
                FirstWord = dto.Word,
                SecondWord = dto.Translation
            }).ToList();
        }
        catch
        {
            return null;
        }
    }

    private string? GetUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}