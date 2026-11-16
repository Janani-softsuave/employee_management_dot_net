namespace EmployeeManagement.Application.DTOs.Employees;

public record EmployeeDto(
    int Id,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    DateOnly HireDate,
    decimal Salary,
    bool IsActive,
    int DepartmentId,
    string? DepartmentName);
