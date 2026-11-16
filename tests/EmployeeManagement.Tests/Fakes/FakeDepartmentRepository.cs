using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Tests.Fakes;

public class FakeDepartmentRepository : IDepartmentRepository
{
    private int _nextId = 1;

    public List<Department> Departments { get; } = new();

    public FakeEmployeeRepository? EmployeeRepository { get; set; }

    public Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Departments.FirstOrDefault(d => d.Id == id));
    }

    public Task AddAsync(Department entity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        entity.Id = _nextId++;
        Departments.Add(entity);
        return Task.CompletedTask;
    }

    public void Update(Department entity)
    {
    }

    public void Remove(Department entity) => Departments.Remove(entity);

    public Task<IReadOnlyList<Department>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Department>>(Departments.OrderBy(d => d.Name).ToList());

    public Task<bool> NameExistsAsync(string name, int? excludeDepartmentId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Departments.Any(d =>
            string.Equals(d.Name, name, StringComparison.OrdinalIgnoreCase) && d.Id != excludeDepartmentId));

    public Task<bool> HasEmployeesAsync(int departmentId, CancellationToken cancellationToken = default) =>
        Task.FromResult(EmployeeRepository?.Employees.Any(e => e.DepartmentId == departmentId) ?? false);
}
