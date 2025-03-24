namespace Backend.Models;

public class BlindedWord
{
    public int Id { get; set; }
    public int BlindedTestId { get; set; } 
    public string Word { get; set; } = string.Empty;
    public BlindedTest BlindedTest { get; set; } = null!;
}