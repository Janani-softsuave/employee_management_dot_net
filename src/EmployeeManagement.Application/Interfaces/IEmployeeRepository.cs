using EmployeeManagement.Application.DTOs.Employees;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Interfaces;

public interface IEmployeeRepository : IRepository<Employee>
{

    Task<(IReadOnlyList<Employee> Items, int TotalCount)> GetPagedAsync(
        EmployeeQueryParameters query, CancellationToken cancellationToken = default);

    Task<Employee?> GetByIdWithDepartmentAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, int? excludeEmployeeId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Employee>> GetAllWithDepartmentAsync(CancellationToken cancellationToken = default);
}
