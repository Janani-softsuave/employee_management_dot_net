using EmployeeManagement.Application.DTOs.Departments;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Mappings;

public static class DepartmentMappingExtensions
{
    public static DepartmentDto ToDto(this Department department) =>
        new(department.Id, department.Name, department.Description);

    public static Department ToEntity(this CreateDepartmentDto dto) =>
        new()
        {
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim()
        };
}
