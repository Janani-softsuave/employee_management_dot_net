namespace EmployeeManagement.Application.DTOs.Departments;

public record DepartmentSummaryDto(int DepartmentId, string DepartmentName, int EmployeeCount, decimal AverageSalary);
