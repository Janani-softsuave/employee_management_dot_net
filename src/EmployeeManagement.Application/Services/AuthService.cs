using EmployeeManagement.Application.DTOs.Auth;
using EmployeeManagement.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserStore _userStore;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUserStore userStore, ITokenService tokenService, ILogger<AuthService> logger)
    {
        _userStore = userStore;
        _tokenService = tokenService;
        _logger = logger;
    }

    public LoginResponseDto Login(LoginRequestDto request)
    {
        var user = _userStore.ValidateCredentials(request.Username, request.Password);

        if (user is null)
        {
            // Never log the password; the username alone is enough for diagnostics.
            _logger.LogWarning("Failed login attempt for user {Username}", request.Username);
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        _logger.LogInformation("User {Username} logged in with role {Role}", user.Username, user.Role);

        return _tokenService.CreateToken(user);
    }
}
