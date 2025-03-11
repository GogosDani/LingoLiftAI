namespace Backend.Models;

public class WordPair
{
    public int Id { get; init; }
    public int SetId { get; init; }
    public string FirstWord { get; init; }
    public string SecondWord { get; init; }
}