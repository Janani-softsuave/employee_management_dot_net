using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Application.DTOs.Employees;

public record UpdateEmployeeDto
{
    [Required, StringLength(50, MinimumLength = 1)]
    public string FirstName { get; init; } = string.Empty;

    [Required, StringLength(50, MinimumLength = 1)]
    public string LastName { get; init; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; init; } = string.Empty;

    [Required]
    public DateOnly HireDate { get; init; }

    [Range(0.01, 10_000_000)]
    public decimal Salary { get; init; }

    [Range(1, int.MaxValue)]
    public int DepartmentId { get; init; }
}
