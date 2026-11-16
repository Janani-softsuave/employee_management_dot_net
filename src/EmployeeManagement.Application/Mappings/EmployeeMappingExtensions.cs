using EmployeeManagement.Application.DTOs.Employees;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Mappings;

/// <summary>Manual entity &lt;-&gt; DTO mapping implemented as extension methods.</summary>
public static class EmployeeMappingExtensions
{
    public static EmployeeDto ToDto(this Employee employee) =>
        new(
            employee.Id,
            employee.FirstName,
            employee.LastName,
            employee.FullName,
            employee.Email,
            employee.HireDate,
            employee.Salary,
            employee.IsActive,
            employee.DepartmentId,
            employee.Department?.Name);

    public static Employee ToEntity(this CreateEmployeeDto dto) =>
        new()
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = dto.Email.Trim().ToLowerInvariant(),
            HireDate = dto.HireDate,
            Salary = dto.Salary,
            DepartmentId = dto.DepartmentId,
            IsActive = true
        };

    public static void ApplyTo(this UpdateEmployeeDto dto, Employee employee)
    {
        employee.FirstName = dto.FirstName.Trim();
        employee.LastName = dto.LastName.Trim();
        employee.Email = dto.Email.Trim().ToLowerInvariant();
        employee.HireDate = dto.HireDate;
        employee.Salary = dto.Salary;
        employee.DepartmentId = dto.DepartmentId;
    }
}
