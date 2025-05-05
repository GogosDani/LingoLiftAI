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

    public WordsetController(IWordsetRepository wordsetRepository)
    {
        _wordsetRepository = wordsetRepository ?? throw new ArgumentNullException(nameof(wordsetRepository));
    }

    [HttpPost]
    public async Task<IActionResult> CreateWordset([FromBody] string setName, int firstLanguageId, int secondLanguageId)
    {
        try
        {
            if (!HttpContext.Request.Cookies.TryGetValue("jwt", out var token))
            {
                return null;
            }
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
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
}