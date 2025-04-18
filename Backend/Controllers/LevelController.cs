using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Route("api/level")]
[ApiController]
public class LevelController : ControllerBase
{
    private readonly ILevelRepository _repository;
    public LevelController(ILevelRepository repository)
    {
        _repository = repository;
    }
    
    [HttpGet("{languageId}"), Authorize]
    public async Task<ActionResult> GetUserLevel(int languageId)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(HttpContext.Request.Cookies["jwt"]);
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var level = _repository.GetUserLevelByLanguageId(languageId, userId);
            return Ok(new { level = level });
        } 
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync(ex.Message);
            return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
        }
    }
}