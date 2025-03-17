using System.Text.Json.Serialization;

namespace Backend.DTOs.AIDTOs;

public class ReadingTestResponse
{
    [JsonPropertyName("story")]
    public string Story { get; set; }
    [JsonPropertyName("questions")]
    public string[] Questions { get; set; }
}