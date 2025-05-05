namespace Backend.Models;

public class CustomSet
{
    public int Id { get; init; }
    public string UserId { get; init; }
    public string Name { get; init; }
    public int FirstLanguageId { get; init; }
    public int SecondLanguageId { get; init; }
    public ICollection<WordPair> WordPairs { get; set; } = new List<WordPair>();
}