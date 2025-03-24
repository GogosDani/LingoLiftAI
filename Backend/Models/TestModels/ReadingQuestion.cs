namespace Backend.Models;

public class ReadingQuestion
{
    public int Id { get; set; }
    public int ReadingTestId { get; set; } 
    public string QuestionText { get; set; }
    public ReadingTest ReadingTest { get; set; } = null!;
}