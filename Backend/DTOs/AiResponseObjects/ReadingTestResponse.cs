using System.Text.Json.Serialization;

namespace Backend.DTOs.AIDTOs;

public class ReadingTestResponse
{
    [JsonPropertyName("text")]
    public string Text { get; set; }
    [JsonPropertyName("questions")]
    public string[] Questions { get; set; }
}