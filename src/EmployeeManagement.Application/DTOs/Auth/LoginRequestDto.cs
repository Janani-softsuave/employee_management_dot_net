using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Application.DTOs.Auth;

public record LoginRequestDto
{
    [Required]
    public string Username { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}
