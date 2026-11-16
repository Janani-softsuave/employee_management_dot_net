using EmployeeManagement.Application.DTOs.Employees;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Tests.Builders;

/// <summary>Small helpers that create valid objects so each test only states what matters to it.</summary>
public static class TestData
{
    public static Employee Employee(
        int id = 0,
        string email = "jane.doe@example.com",
        int departmentId = 1,
        decimal salary = 50000m,
        bool isActive = true,
        string firstName = "Jane",
        string lastName = "Doe",
        DateOnly? hireDate = null) =>
        new()
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            DepartmentId = departmentId,
            Salary = salary,
            IsActive = isActive,
            HireDate = hireDate ?? new DateOnly(2024, 1, 15)
        };

    public static CreateEmployeeDto CreateEmployeeDto(
        string email = "new.hire@example.com",
        int departmentId = 1,
        DateOnly? hireDate = null) =>
        new()
        {
            FirstName = "New",
            LastName = "Hire",
            Email = email,
            DepartmentId = departmentId,
            Salary = 55000m,
            HireDate = hireDate ?? new DateOnly(2026, 1, 10)
        };

    public static UpdateEmployeeDto UpdateEmployeeDto(
        string email = "updated@example.com",
        int departmentId = 1,
        DateOnly? hireDate = null) =>
        new()
        {
            FirstName = "Updated",
            LastName = "Person",
            Email = email,
            DepartmentId = departmentId,
            Salary = 60000m,
            HireDate = hireDate ?? new DateOnly(2025, 5, 5)
        };
}
