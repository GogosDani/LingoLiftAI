namespace Backend.DTOs;

public class ResetPwdRequest
{
    public string Token { get; init; }
    public string Email { get; init; }
    public string NewPassword { get; init; }
}