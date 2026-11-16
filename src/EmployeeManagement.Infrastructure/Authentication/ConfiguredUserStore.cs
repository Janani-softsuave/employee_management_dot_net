using System.Security.Cryptography;
using System.Text;
using EmployeeManagement.Application.DTOs.Auth;
using EmployeeManagement.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace EmployeeManagement.Infrastructure.Authentication;

/// <summary>Looks users up in configuration. Swap for a database-backed store in a real system.</summary>
public class ConfiguredUserStore : IUserStore
{
    private readonly DemoUserSettings _settings;

    public ConfiguredUserStore(IOptions<DemoUserSettings> options)
    {
        _settings = options.Value;
    }

    public AuthenticatedUser? ValidateCredentials(string username, string password)
    {
        var user = _settings.Users.FirstOrDefault(
            u => string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));

        if (user is null)
        {
            return null;
        }

        // Constant-time comparison avoids leaking information through timing.
        var expected = Encoding.UTF8.GetBytes(user.Password);
        var actual = Encoding.UTF8.GetBytes(password);

        return CryptographicOperations.FixedTimeEquals(expected, actual)
            ? new AuthenticatedUser(user.Username, user.Role)
            : null;
    }
}
