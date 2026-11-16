using EmployeeManagement.Application.DTOs.Common;
using EmployeeManagement.Application.DTOs.Employees;

namespace EmployeeManagement.Application.Interfaces;

public interface IEmployeeService
{
    Task<PagedResult<EmployeeDto>> GetEmployeesAsync(EmployeeQueryParameters query, CancellationToken cancellationToken = default);

    Task<EmployeeDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto, CancellationToken cancellationToken = default);

    Task UpdateAsync(int id, UpdateEmployeeDto dto, CancellationToken cancellationToken = default);

    Task UpdateStatusAsync(int id, UpdateEmployeeStatusDto dto, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
