namespace Backend.DTOs.Challenge;

public class UserChallengeResultDto
{
    public string UserId { get; set; }
    public int Score { get; set; }
    public DateTime CompletedAt { get; set; }
    public string Username { get; set; }
}