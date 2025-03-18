namespace Backend.DTOs.AIDTOs;

public record BlindedResultRequest(int LanguageId, string[] CorrectAnswers, string[] UserAnswers);