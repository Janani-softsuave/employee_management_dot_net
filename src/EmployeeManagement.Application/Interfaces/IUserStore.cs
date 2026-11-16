using EmployeeManagement.Application.DTOs.Auth;

namespace EmployeeManagement.Application.Interfaces;

public interface IUserStore
{
    /// <summary>Returns the user when the credentials are valid, otherwise null.</summary>
    AuthenticatedUser? ValidateCredentials(string username, string password);
}
