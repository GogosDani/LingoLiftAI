namespace Backend.DTOs.ChallengeDtos;

public class UserAnswerSubmissionDto
{
    public List<string> Answers { get; set; } = new();
    public string UserId { get; set; } = string.Empty;
}