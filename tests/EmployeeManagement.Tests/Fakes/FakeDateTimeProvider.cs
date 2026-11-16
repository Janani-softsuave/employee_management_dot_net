using EmployeeManagement.Application.Interfaces;

namespace EmployeeManagement.Tests.Fakes;

public class FakeDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow { get; init; } = new(2026, 6, 15, 12, 0, 0, DateTimeKind.Utc);

    public DateOnly Today => DateOnly.FromDateTime(UtcNow);
}
