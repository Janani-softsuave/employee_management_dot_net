namespace EmployeeManagement.ConsoleApp.Models;

/// <summary>
/// Reference type (class): variables hold a reference to one object on the heap.
/// Assigning it to another variable copies the reference, not the data.
/// </summary>
public class EmployeeRecord
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Department { get; init; } = string.Empty;

    public decimal Salary { get; init; }

    /// <summary>Nullable value type: <c>null</c> means this employee has no manager.</summary>
    public int? ManagerId { get; init; }
}
