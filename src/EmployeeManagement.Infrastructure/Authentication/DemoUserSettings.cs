namespace EmployeeManagement.Infrastructure.Authentication;

/// <summary>Demo users bound from the "DemoUsers" configuration section (development only).</summary>
public class DemoUserSettings
{
    public const string SectionName = "DemoUsers";

    public List<DemoUser> Users { get; set; } = new();
}

public class DemoUser
{
    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}
