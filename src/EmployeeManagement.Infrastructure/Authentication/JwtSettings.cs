namespace EmployeeManagement.Infrastructure.Authentication;

/// <summary>Bound from the "Jwt" configuration section (options pattern). The key is never committed.</summary>
public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    /// <summary>Signing key. Supplied via user-secrets or an environment variable, at least 32 characters.</summary>
    public string Key { get; set; } = string.Empty;

    public int ExpiryMinutes { get; set; } = 60;
}
