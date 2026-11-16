namespace EmployeeManagement.ConsoleApp.Models;

public class EmployeeRecord
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Department { get; init; } = string.Empty;

    public decimal Salary { get; init; }

    public int? ManagerId { get; init; }
}
