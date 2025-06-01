namespace Backend.Models;

public class Word
{
    public int Id { get; set; }
    public string FirstLang { get; set; } = string.Empty;
    public string SecondLang { get; set; } = string.Empty;
    public virtual AiWordSet WordSet { get; set; } = null!;
}