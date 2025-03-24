using Backend.Models;

namespace Backend.Services.Repositories;

public interface ILanguageRepository
{
    public Task<IEnumerable<Language>> GetAllLanguages();
}