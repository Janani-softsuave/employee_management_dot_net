using EmployeeManagement.Application.DTOs.Auth;

namespace EmployeeManagement.Application.Interfaces;

public interface IAuthService
{
    /// <summary>Validates credentials and returns a JWT. Throws UnauthorizedAccessException when invalid.</summary>
    LoginResponseDto Login(LoginRequestDto request);
}
