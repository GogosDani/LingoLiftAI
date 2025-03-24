namespace Backend.Models;

public class BlindedCorrect
{
    public int Id { get; set; }
    public int BlindedTestId { get; set; } 
    public string Correct { get; set; } = string.Empty;
    public BlindedTest BlindedTest { get; set; } = null!;
}