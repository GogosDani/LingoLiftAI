namespace Backend.Models;

public class DailyChallenge
{
    public int Id { get; init; }
    public DateTime Date { get; init; }
    public string Content { get; init; }
    public ChallengeType Type { get; init; }
    public ICollection<UserChallenge> UserChallenges { get; set; } = new List<UserChallenge>();
}