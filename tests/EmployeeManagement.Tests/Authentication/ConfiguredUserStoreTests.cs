using EmployeeManagement.Infrastructure.Authentication;
using Microsoft.Extensions.Options;

namespace EmployeeManagement.Tests.Authentication;

public class ConfiguredUserStoreTests
{
    private readonly ConfiguredUserStore _sut = new(Options.Create(new DemoUserSettings
    {
        Users =
        [
            new DemoUser { Username = "admin", Password = "pw-admin", Role = "Admin" },
            new DemoUser { Username = "user", Password = "pw-user", Role = "User" }
        ]
    }));

    [Fact]
    public void ValidateCredentials_CorrectPassword_ReturnsUserWithRole()
    {
        var user = _sut.ValidateCredentials("admin", "pw-admin");

        Assert.NotNull(user);
        Assert.Equal("Admin", user.Role);
    }

    [Fact]
    public void ValidateCredentials_UsernameIsCaseInsensitive()
    {
        var user = _sut.ValidateCredentials("USER", "pw-user");

        Assert.NotNull(user);
    }

    [Fact]
    public void ValidateCredentials_PasswordIsCaseSensitive()
    {
        Assert.Null(_sut.ValidateCredentials("user", "PW-USER"));
    }

    [Fact]
    public void ValidateCredentials_UnknownUser_ReturnsNull()
    {
        Assert.Null(_sut.ValidateCredentials("ghost", "pw-admin"));
    }
}
