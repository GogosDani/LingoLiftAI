namespace Backend.Models;

public class CorrectionSentence
{
    public int Id { get; set; }
    public int CorrectionTestId { get; set; } 
    public string Word { get; set; } = string.Empty;
    public CorrectionTest CorrectionTest { get; set; } = null!;
}