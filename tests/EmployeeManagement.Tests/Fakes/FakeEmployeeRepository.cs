using EmployeeManagement.Application.DTOs.Employees;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Tests.Fakes;

/// <summary>In-memory fake so service logic can be tested without a database.</summary>
public class FakeEmployeeRepository : IEmployeeRepository
{
    private int _nextId = 1;

    public List<Employee> Employees { get; } = new();

    public FakeDepartmentRepository? Departments { get; init; }

    public Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Employees.FirstOrDefault(e => e.Id == id));
    }

    public Task<Employee?> GetByIdWithDepartmentAsync(int id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var employee = Employees.FirstOrDefault(e => e.Id == id);
        AttachDepartment(employee);
        return Task.FromResult(employee);
    }

    public Task AddAsync(Employee entity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        entity.Id = _nextId++;
        Employees.Add(entity);
        return Task.CompletedTask;
    }

    public void Update(Employee entity)
    {
        // Entities are held by reference, so changes are already visible.
    }

    public void Remove(Employee entity) => Employees.Remove(entity);

    public Task<bool> EmailExistsAsync(string email, int? excludeEmployeeId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Employees.Any(e => e.Email == email && e.Id != excludeEmployeeId));
    }

    public Task<IReadOnlyList<Employee>> GetAllWithDepartmentAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Employees.ForEach(AttachDepartment);
        return Task.FromResult<IReadOnlyList<Employee>>(Employees.ToList());
    }

    public Task<(IReadOnlyList<Employee> Items, int TotalCount)> GetPagedAsync(
        EmployeeQueryParameters query, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<Employee> items = Employees.ToList();
        return Task.FromResult((items, Employees.Count));
    }

    private void AttachDepartment(Employee? employee)
    {
        if (employee is not null && Departments is not null)
        {
            employee.Department = Departments.Departments.FirstOrDefault(d => d.Id == employee.DepartmentId);
        }
    }
}
