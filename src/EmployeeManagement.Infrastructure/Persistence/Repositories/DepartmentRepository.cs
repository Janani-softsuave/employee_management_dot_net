using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Persistence.Repositories;

public class DepartmentRepository : Repository<Department>, IDepartmentRepository
{
    public DepartmentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Department>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await Context.Departments
            .AsNoTracking()
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);

    public async Task<bool> NameExistsAsync(string name, int? excludeDepartmentId, CancellationToken cancellationToken = default)
    {
        var normalized = name.ToLower();

        return await Context.Departments.AnyAsync(
            d => d.Name.ToLower() == normalized && (excludeDepartmentId == null || d.Id != excludeDepartmentId),
            cancellationToken);
    }

    public async Task<bool> HasEmployeesAsync(int departmentId, CancellationToken cancellationToken = default) =>
        await Context.Employees.AnyAsync(e => e.DepartmentId == departmentId, cancellationToken);
}
