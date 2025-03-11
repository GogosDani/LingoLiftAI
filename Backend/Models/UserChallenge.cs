namespace Backend.Models;

public class UserChallenge
{
    public int Id { get; init; }
    public string UserId { get; init; }
    public int DailyChallengeId { get; init; }
    public int Score { get; init; }
    public int Seconds { get; init; }
    public int Points {get; init;}
}