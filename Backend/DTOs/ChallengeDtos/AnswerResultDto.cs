namespace Backend.DTOs.ChallengeDtos;

public class AnswerResultDto
{
    public int QuestionNumber { get; set; }
    public string UserAnswer { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public string Feedback { get; set; } = string.Empty;
}
