namespace Backend.Models;

public class WritingQuestion
{
    public int Id { get; set; }
    public int WritingQuestionsId { get; set; } 
    public string QuestionText { get; set; }
    public WritingQuestions WritingQuestions { get; set; } = null!;
}