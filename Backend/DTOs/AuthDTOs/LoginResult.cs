namespace Backend.DTOs;

public record LoginResult(bool Success, string ErrorMessage, string? Token);