namespace Backend.Models;

public class CorrectionTest
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public List<CorrectionSentence> Sentences { get; set; } = new();
}