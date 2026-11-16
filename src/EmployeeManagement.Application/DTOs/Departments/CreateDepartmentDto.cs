using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Application.DTOs.Departments;

public record CreateDepartmentDto
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; init; }
}
