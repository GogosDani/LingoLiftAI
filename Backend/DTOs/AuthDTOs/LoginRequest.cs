namespace Backend.DTOs;

public record LoginRequest(
    string Email,
    string Password
);