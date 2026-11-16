namespace EmployeeManagement.Application.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }

    DateOnly Today { get; }
}
