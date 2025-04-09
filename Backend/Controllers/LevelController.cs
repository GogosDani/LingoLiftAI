using Backend.Services.Repositories;
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
}