using EmployeeManagement.Application.DTOs.Auth;

namespace EmployeeManagement.Application.Interfaces;

public interface IUserStore
{

    AuthenticatedUser? ValidateCredentials(string username, string password);
}
