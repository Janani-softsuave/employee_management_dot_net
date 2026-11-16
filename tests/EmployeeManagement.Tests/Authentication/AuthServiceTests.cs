using EmployeeManagement.Application.DTOs.Auth;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Application.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace EmployeeManagement.Tests.Authentication;

public class AuthServiceTests
{
    private sealed class StubUserStore : IUserStore
    {
        public AuthenticatedUser? ValidateCredentials(string username, string password) =>
            username == "admin" && password == "secret" ? new AuthenticatedUser("admin", "Admin") : null;
    }

    private sealed class StubTokenService : ITokenService
    {
        public LoginResponseDto CreateToken(AuthenticatedUser user) =>
            new("token-for-" + user.Username, DateTime.UtcNow.AddHours(1), user.Username, user.Role);
    }

    private readonly AuthService _sut = new(new StubUserStore(), new StubTokenService(), NullLogger<AuthService>.Instance);

    [Fact]
    public void Login_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var request = new LoginRequestDto { Username = "admin", Password = "secret" };

        // Act
        var response = _sut.Login(request);

        // Assert
        Assert.Equal("token-for-admin", response.AccessToken);
        Assert.Equal("Admin", response.Role);
    }

    [Theory]
    [InlineData("admin", "wrong")]
    [InlineData("nobody", "secret")]
    [InlineData("", "")]
    public void Login_InvalidCredentials_ThrowsUnauthorizedAccessException(string username, string password)
    {
        var request = new LoginRequestDto { Username = username, Password = password };

        Assert.Throws<UnauthorizedAccessException>(() => _sut.Login(request));
    }
}
