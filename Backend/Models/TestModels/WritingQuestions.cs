namespace Backend.Models;

public class WritingQuestions
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public List<WritingQuestion> Questions { get; set; } = new();
}