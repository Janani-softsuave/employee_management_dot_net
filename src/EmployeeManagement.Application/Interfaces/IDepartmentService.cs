using EmployeeManagement.Application.DTOs.Departments;

namespace EmployeeManagement.Application.Interfaces;

public interface IDepartmentService
{
    Task<IReadOnlyList<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<DepartmentDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto, CancellationToken cancellationToken = default);

    Task UpdateAsync(int id, UpdateDepartmentDto dto, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DepartmentSummaryDto>> GetSummariesAsync(CancellationToken cancellationToken = default);
}
