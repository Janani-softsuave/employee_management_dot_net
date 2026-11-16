using EmployeeManagement.Application.DTOs.Employees;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Persistence.Repositories;

public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<(IReadOnlyList<Employee> Items, int TotalCount)> GetPagedAsync(
        EmployeeQueryParameters query, CancellationToken cancellationToken = default)
    {
        // AsNoTracking: read-only query, so EF Core skips change-tracking overhead.
        var filtered = Context.Employees
            .AsNoTracking()
            .ApplyFilters(query);

        var totalCount = await filtered.CountAsync(cancellationToken);

        // Eager loading: Include joins the department so we avoid an N+1 query pattern.
        var items = await filtered
            .Include(e => e.Department)
            .ApplySorting(query)
            .ApplyPaging(query)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Employee?> GetByIdWithDepartmentAsync(int id, CancellationToken cancellationToken = default) =>
        await Context.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<bool> EmailExistsAsync(string email, int? excludeEmployeeId, CancellationToken cancellationToken = default) =>
        await Context.Employees.AnyAsync(
            e => e.Email == email && (excludeEmployeeId == null || e.Id != excludeEmployeeId),
            cancellationToken);

    public async Task<IReadOnlyList<Employee>> GetAllWithDepartmentAsync(CancellationToken cancellationToken = default) =>
        await Context.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .ToListAsync(cancellationToken);
}
