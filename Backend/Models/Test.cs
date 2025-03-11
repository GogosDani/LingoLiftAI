namespace Backend.Models;

public class Test
{
    public int Id { get; init; }
    public string UserId { get; init; }
    public int LevelId { get; init; }
    public Level Level { get; init; }
    public Language Language { get; init; }
    public int LanguageId { get; init; }
    public int Score { get; init; }
    public DateTime CompletedAt { get; init; }
    public int SetId { get; init; }
}