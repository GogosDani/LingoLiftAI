namespace Backend.DTOs.AIDTOs;

public record WritingResultRequest(int LanguageId, string[] Questions, string[] Answers);