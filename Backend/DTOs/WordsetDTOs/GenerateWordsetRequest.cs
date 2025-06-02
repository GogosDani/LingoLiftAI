namespace Backend.DTOs.WordsetDTOs;

public class GenerateWordsetRequest
{
    public int TopicId { get; set; }
    public string DifficultyLevel { get; set; } = string.Empty;
    public int WordCount { get; set; }
}