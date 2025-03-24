namespace Backend.DTOs.AIDTOs;

public record BlindedResultRequest(int LanguageId, int BlindedTestId, string[] UserAnswers);