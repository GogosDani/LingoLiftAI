namespace Backend.DTOs.ChallengeDtos;

public class UserAnswerEvaluationDto
{
    public int TotalScore { get; set; }
    public int MaxScore { get; set; }
    public List<AnswerResultDto> Results { get; set; } = new();
    public string OverallFeedback { get; set; } = string.Empty;
}