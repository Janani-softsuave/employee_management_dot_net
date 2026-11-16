namespace EmployeeManagement.Application.Interfaces;

/// <summary>Abstraction over the clock so time-dependent rules are deterministic in tests.</summary>
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }

    DateOnly Today { get; }
}
