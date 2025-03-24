using System.Text.Json.Serialization;

namespace Backend.DTOs.AIDTOs;

public class ReadingResultRequest
{
   public int ReadingTestId { get; set; }
    public string[] Answers { get; set; }
    public int LanguageId { get; set; }
}