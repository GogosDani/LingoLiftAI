using System.Text.Json.Serialization;

namespace Backend.DTOs.ChallengeDtos;

public class ChallengeGenerationResponse
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }
}