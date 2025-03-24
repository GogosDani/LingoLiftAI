namespace Backend.Services.AIServices;

public interface IAIClient
{
    public Task<string> GetAiAnswer(string prompt);
}