using EmployeeManagement.Application.DTOs.Auth;

namespace EmployeeManagement.Application.Interfaces;

public interface ITokenService
{
    LoginResponseDto CreateToken(AuthenticatedUser user);
}
