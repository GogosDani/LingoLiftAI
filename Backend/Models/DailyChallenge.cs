namespace Backend.Models;

public class DailyChallenge
{
    public int Id { get; init; }
    public DateOnly Date { get; init; }
    public string QuestionData { get; init; }
    public ICollection<UserChallenge> UserChallenges { get; set; } = new List<UserChallenge>();
}