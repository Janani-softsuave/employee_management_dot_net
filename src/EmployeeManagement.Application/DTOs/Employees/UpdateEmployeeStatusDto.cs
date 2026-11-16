namespace EmployeeManagement.Application.DTOs.Employees;

/// <summary>Partial update of an employee (PATCH semantics): activate or deactivate.</summary>
public record UpdateEmployeeStatusDto(bool IsActive);
