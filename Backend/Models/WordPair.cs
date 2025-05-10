namespace Backend.Models;

public class WordPair
{
    public int Id { get; init; }
    public int SetId { get; init; }
    public string FirstWord { get; set; }
    public string SecondWord { get; set; }
    public CustomSet Set { get; set; }
}
