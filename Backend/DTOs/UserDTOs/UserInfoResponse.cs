namespace Backend.DTOs.UserDTOs;

public record UserInfoResponse(
    string Email,
    string Username,
    string Id
);