using System.Text.Json.Serialization;

namespace Backend.DTOs.AIDTOs;

public class CorrectionResponse
{
    [JsonPropertyName("sentences")]
    public string[] Sentences{ get; set; }
    [JsonPropertyName("answers")]
    public string[] Answers { get; set; }
}