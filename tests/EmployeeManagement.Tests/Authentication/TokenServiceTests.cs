using System.IdentityModel.Tokens.Jwt;
using EmployeeManagement.Application.DTOs.Auth;
using EmployeeManagement.Infrastructure.Authentication;
using EmployeeManagement.Tests.Fakes;
using Microsoft.Extensions.Options;

namespace EmployeeManagement.Tests.Authentication;

public class TokenServiceTests
{
    private static TokenService CreateSut(FakeDateTimeProvider clock) =>
        new(
            Options.Create(new JwtSettings
            {
                Issuer = "test-issuer",
                Audience = "test-audience",
                Key = "unit-test-signing-key-0123456789-abcdefghij",
                ExpiryMinutes = 30
            }),
            clock);

    [Fact]
    public void CreateToken_ContainsNameAndRoleClaims()
    {
        // Arrange
        var sut = CreateSut(new FakeDateTimeProvider());

        // Act
        var response = sut.CreateToken(new AuthenticatedUser("admin", "Admin"));

        // Assert
        var token = new JwtSecurityTokenHandler().ReadJwtToken(response.AccessToken);
        Assert.Equal("Admin", token.Claims.Single(c => c.Type == ClaimNames.Role).Value);
        Assert.Equal("admin", token.Claims.Single(c => c.Type == ClaimNames.Name).Value);
        Assert.Equal("test-issuer", token.Issuer);
    }

    [Fact]
    public void CreateToken_ExpiresAfterConfiguredMinutes()
    {
        // Arrange
        var clock = new FakeDateTimeProvider();
        var sut = CreateSut(clock);

        // Act
        var response = sut.CreateToken(new AuthenticatedUser("user", "User"));

        // Assert
        Assert.Equal(clock.UtcNow.AddMinutes(30), response.ExpiresAtUtc);
    }

    [Fact]
    public void CreateToken_DoesNotContainPassword()
    {
        var sut = CreateSut(new FakeDateTimeProvider());

        var response = sut.CreateToken(new AuthenticatedUser("user", "User"));

        var token = new JwtSecurityTokenHandler().ReadJwtToken(response.AccessToken);
        Assert.DoesNotContain(token.Claims, c => c.Type.Contains("password", StringComparison.OrdinalIgnoreCase));
    }
}
