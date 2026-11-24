namespace EmployeeManagement.Infrastructure.Authentication;

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
