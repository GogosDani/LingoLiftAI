using System.Text.Json.Serialization;

namespace Backend.DTOs.AIDTOs;

public class BlindedStoryResponse
{
    [JsonPropertyName("story")]
    public string Story { get; set; }
    [JsonPropertyName("words")]
    public string[] Words { get; set; }
    [JsonPropertyName("answers")]
    public string[] Answers { get; set; }
}