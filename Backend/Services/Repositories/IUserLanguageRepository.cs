using Backend.Models;

namespace Backend.Services.Repositories;

public interface IUserLanguageRepository
{
    public Task<bool> UserHasLevel(string userId);
    public Task AddUserLanguageLevel(string userId, int languageId, string level);
    public Task ChangeLanguageLevel(string userId, int languageId, string level);
    public Task<string> GetUserLanguageLevel(string userId, int languageId);
    public Task<string> GetPreviousLevel(string level);
    public Task<string> GetNextLevel(string level);
    public Task<Language> GetLanguageById(int id);
    public Task<int> AddWritingQuestions(string[] questions, string userId);
    public Task<IEnumerable<WritingQuestion>> GetWritingQuestions(int questionSetId);
    public Task<int> AddReadingTest(string userId, string story, string[] questions);
    public Task<ReadingTest> GetReadingTest(int id);
    public Task<int> AddBlindedTest(string userId, string story, string[] words, string[] corrects);
    public Task<BlindedTest> GetBlindedTest(int id);
    public Task<int> AddCorrection(string[] sentences, string userId);
    public Task<CorrectionTest> GetCorrectionTest(int id);
}