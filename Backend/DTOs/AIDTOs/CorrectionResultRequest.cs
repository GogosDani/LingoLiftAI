namespace Backend.DTOs.AIDTOs;

public record CorrectionResultRequest(string[] Answers, string[] Sentences, int LanguageId);