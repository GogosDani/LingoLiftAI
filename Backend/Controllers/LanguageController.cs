using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend.DTOs;
using Backend.Models;
using Backend.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Route("api/language")]
[ApiController]
public class LanguageController : ControllerBase
{
    private readonly ILanguageRepository _repository;
    private readonly IUserLanguageRepository _userLanguageRepository;

    public LanguageController(ILanguageRepository repository, IUserLanguageRepository userLanguageRepository)
    {
        _repository = repository;
        _userLanguageRepository = userLanguageRepository;
    }

    [HttpGet, Authorize]
    public async Task<IActionResult> GetLanguages()
    {
        try
        {
            return Ok(await _repository.GetAllLanguages());
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost, Authorize]
    public async Task<IActionResult> AddUserLanguageBeginner([FromBody] UserLanguageRequest request)
    {
        try
        {
            var token = HttpContext.Request.Cookies["jwt"];
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            await _userLanguageRepository.AddUserLanguageLevel(userId, request.LanguageId, "Beginner");
            return Ok("Language added to user successfully!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}