namespace EmployeeManagement.Application.DTOs.Auth;

public record LoginResponseDto(string AccessToken, DateTime ExpiresAtUtc, string Username, string Role);
