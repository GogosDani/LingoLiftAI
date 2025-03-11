namespace Backend.DTOs;

public record RegistrationRequest(
    string Email,
    string Username,
    string Password
    );