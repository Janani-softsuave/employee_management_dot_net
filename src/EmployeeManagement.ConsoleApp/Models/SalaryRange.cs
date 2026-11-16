namespace EmployeeManagement.ConsoleApp.Models;

public readonly record struct SalaryRange(decimal Min, decimal Max)
{
    public bool Contains(decimal salary) => salary >= Min && salary <= Max;
}
