namespace EmployeeManagement.Application.DTOs.Departments;

/// <summary>Result of a LINQ GroupBy/Select over employees.</summary>
public record DepartmentSummaryDto(int DepartmentId, string DepartmentName, int EmployeeCount, decimal AverageSalary);
