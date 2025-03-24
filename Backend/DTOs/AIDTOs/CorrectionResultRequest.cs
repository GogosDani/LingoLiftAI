namespace Backend.DTOs.AIDTOs;

public record CorrectionResultRequest(string[] Answers, int CorrectionId, int LanguageId);