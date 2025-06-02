using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Backend.DTOs.WordsetDTOs;
using Backend.Services.Repositories;

namespace Backend.Controllers;

[Route("api/wordset")]
[ApiController]
public class WordsetController : ControllerBase
{
    private readonly IWordsetRepository _wordsetRepository;
    private readonly ILanguageRepository _languageRepository;

    public WordsetController(IWordsetRepository wordsetRepository, ILanguageRepository languageRepository)
    {
        _wordsetRepository = wordsetRepository ?? throw new ArgumentNullException(nameof(wordsetRepository));
        _languageRepository = languageRepository ?? throw new ArgumentNullException(nameof(languageRepository));
    }

    [HttpPost]
    public async Task<IActionResult> CreateWordset([FromBody] string setName, int firstLanguageId, int secondLanguageId)
    {
        try
        {
            var userId = GetUserId();
            int wordsetId = await _wordsetRepository.CreateWordset(
                userId,
                setName, 
                firstLanguageId, 
                secondLanguageId);
            return Ok(wordsetId);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Server error: {ex.Message}");
        }
    }

    [HttpPost("{wordsetId}/wordpair")]
    public async Task<IActionResult> AddWordPair(int wordsetId)
    {
        try
        {
            var wordPair = await _wordsetRepository.AddWordPair(wordsetId);
            return Ok(wordPair.Id);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Server error: {ex.Message}");
        }
    }

    [HttpPut("wordpair/{id}")]
    public async Task<IActionResult> EditWordPair(int id, [FromBody] WordPairModel model)
    {
        try
        {
            var wordPair = await _wordsetRepository.EditWordPair(id, model.FirstWord, model.SecondWord);
            return Ok(wordPair);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Server error: {ex.Message}");
        }
    }

    [HttpDelete("wordpair/{id}")]
    public async Task<IActionResult> DeleteWordPair(int id)
    {
        try
        {
            bool result = await _wordsetRepository.DeleteWordPair(id);
            if (result) return Ok();
            return NotFound($"WordPair {id} not found");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Server error: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWordset(int id)
    {
        try
        {
            bool result = await _wordsetRepository.DeleteWordset(id);
            if (result) return Ok();
            return NotFound($"Wordset {id} not found");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Server error: {ex.Message}");
        }
    }
    [HttpGet]
    public async Task<IActionResult> GetUsersWordsets()
    {
        try
        {
            var userId = GetUserId();
            var response = await _wordsetRepository.GetByUserId(userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Server error: {ex.Message}");
        }
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetWordsetById(int id)
    {
        try
        {
            var userId = GetUserId();
            var set = await _wordsetRepository.GetById(id, userId);
            var firstLanguage = await _languageRepository.GetById(set.FirstLanguageId);
            var secondLanguage = await _languageRepository.GetById(set.SecondLanguageId);
            var wordPairs = set.WordPairs.Select(wp => new WordPairResponse(
                wp.Id,
                wp.FirstWord,
                wp.SecondWord
            )).ToList();
            var wordsetResponse = new WordsetResponse(
                set.Id,
                set.Name,
                firstLanguage,
                secondLanguage,
                wordPairs
            );
            return Ok(wordsetResponse);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Server error: {ex.Message}");
        }
    }
    
    [HttpPut]
    public async Task<IActionResult> EditWordset(EditWordsetRequest request)
    {
        try
        {
            await _wordsetRepository.EditWordset(request.Id, request.Name);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Server error: {ex.Message}");
        }
    }

    private string GetUserId()
    {
        if (!HttpContext.Request.Cookies.TryGetValue("jwt", out var token))
        {
            return null;
        }
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        return jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    }
}