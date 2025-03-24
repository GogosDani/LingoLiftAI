namespace Backend.DTOs.AIDTOs;

public record WritingResultRequest(int LanguageId, string[] Answers, int QuestionSetId);