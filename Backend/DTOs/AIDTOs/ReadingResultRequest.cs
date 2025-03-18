using System.Text.Json.Serialization;

namespace Backend.DTOs.AIDTOs;

public class ReadingResultRequest
{
    public string Text { get; set; }
    public string[] Questions { get; set; }
    public string[] Answers { get; set; }
    public int LanguageId { get; set; }
}