namespace Backend.Models;

public class AiWordSet
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TopicId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string DifficultyLevel { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public virtual Topic Topic { get; set; } = null!;
    public virtual ICollection<AiWordPair> WordPairs { get; set; } = new List<AiWordPair>();
}
