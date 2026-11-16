using EmployeeManagement.Application.DTOs.Auth;

namespace EmployeeManagement.Application.Interfaces;

public interface IAuthService
{

    LoginResponseDto Login(LoginRequestDto request);
}
