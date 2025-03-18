namespace Backend.DTOs.AIDTOs;

public record GeminiResponse(
    Candidate[] candidates
    );