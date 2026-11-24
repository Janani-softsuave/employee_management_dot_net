using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Interfaces;

public interface IDepartmentRepository : IRepository<Department>
{
    Task<IReadOnlyList<Department>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<bool> NameExistsAsync(string name, int? excludeDepartmentId, CancellationToken cancellationToken = default);

    Task<bool> HasEmployeesAsync(int departmentId, CancellationToken cancellationToken = default);
}
