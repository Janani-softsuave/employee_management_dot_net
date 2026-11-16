namespace EmployeeManagement.ConsoleApp.Models;

/// <summary>
/// Value type (struct): the data lives inside the variable itself.
/// Assigning it to another variable copies all of its fields.
/// </summary>
public readonly record struct SalaryRange(decimal Min, decimal Max)
{
    public bool Contains(decimal salary) => salary >= Min && salary <= Max;
}
