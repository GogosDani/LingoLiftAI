using Backend.DTOs.Challenge;
using Backend.Models;
using Backend.Services.ChallengeServices;
using Backend.Services.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChallengeController : ControllerBase
{
    private readonly IChallengeRepository _challengeRepository;
    private readonly IDailyChallengeGeneratorService _challengeGenerator;
    private readonly ILogger<ChallengeController> _logger;

    public ChallengeController(
        IChallengeRepository challengeRepository,
        IDailyChallengeGeneratorService challengeGenerator,
        ILogger<ChallengeController> logger)
    {
        _challengeRepository = challengeRepository;
        _challengeGenerator = challengeGenerator;
        _logger = logger;
    }

    [HttpGet("today")]
    public async Task<ActionResult<DailyChallenge>> GetTodaysChallenge()
    {
        try
        {
            await _challengeGenerator.EnsureTodaysChallengeExistsAsync();
            
            var today = DateTime.Today;
            var challenge = await _challengeRepository.GetChallengeByDateAsync(today);
            
            if (challenge == null)
            {
                return NotFound(new { message = "No challenge found for today" });
            }
            
            return Ok(challenge);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving today's challenge");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("leaderboard")]
    public async Task<ActionResult<List<UserChallengeResultDto>>> GetTodaysLeaderboard()
    {
        try
        {
            var today = DateTime.Today;
            var results = await _challengeRepository.GetTop10UserResultsByDateAsync(today);
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving today's leaderboard");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("generate/{date}")]
    public async Task<ActionResult<DailyChallenge>> GenerateChallengeForDate(DateTime date)
    {
        try
        {
            var challenge = await _challengeGenerator.GenerateChallengeForDateAsync(date);
            return Ok(challenge);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error generating challenge for {date:yyyy-MM-dd}");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}
