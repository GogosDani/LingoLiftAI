namespace Backend.Models;

public class UserChallenge
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public int DailyChallengeId { get; set; }
    public int Score { get; set; }
    public DateTime CompletedAt { get; set; }
    public DailyChallenge? DailyChallenge { get; set; }
    public ApplicationUser? User { get; set; }
}