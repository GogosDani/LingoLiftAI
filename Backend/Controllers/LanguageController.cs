using Backend.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Route("api/language")]
[ApiController]
public class LanguageController : ControllerBase
{
    private readonly ILanguageRepository _repository;

    public LanguageController(ILanguageRepository repository)
    {
        _repository = repository;
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
}