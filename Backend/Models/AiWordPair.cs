namespace Backend.Models;

public class AiWordPair
{
    public int Id { get; set; }
    public int WordSetId { get; set; }
    public string FirstWord { get; set; } = string.Empty;
    public string SecondWord { get; set; } = string.Empty;
    public virtual AiWordSet WordSet { get; set; } = null!;
}