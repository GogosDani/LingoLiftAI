namespace Backend.Models;

public class ReadingTest
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Story { get; set; }
    public List<ReadingQuestion> Questions { get; set; } = new();
}