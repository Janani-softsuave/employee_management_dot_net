using EmployeeManagement.Application.DTOs.Employees;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Infrastructure.Persistence.Extensions;

/// <summary>
/// Reusable filtering, sorting and paging extension methods over IQueryable.
/// They only build the query expression (deferred execution); the database runs it later.
/// </summary>
public static class EmployeeQueryableExtensions
{
    public static IQueryable<Employee> ApplyFilters(this IQueryable<Employee> query, EmployeeQueryParameters parameters)
    {
        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var term = parameters.Search.Trim().ToLower();

            query = query.Where(e =>
                e.FirstName.ToLower().Contains(term) ||
                e.LastName.ToLower().Contains(term) ||
                e.Email.ToLower().Contains(term));
        }

        if (parameters.DepartmentId.HasValue)
        {
            query = query.Where(e => e.DepartmentId == parameters.DepartmentId.Value);
        }

        if (parameters.IsActive.HasValue)
        {
            query = query.Where(e => e.IsActive == parameters.IsActive.Value);
        }

        return query;
    }

    public static IQueryable<Employee> ApplySorting(this IQueryable<Employee> query, EmployeeQueryParameters parameters)
    {
        // Pattern matching selects the sort column; unknown values fall back to last name.
        return (parameters.SortBy?.ToLowerInvariant(), parameters.Descending) switch
        {
            ("firstname", false) => query.OrderBy(e => e.FirstName).ThenBy(e => e.Id),
            ("firstname", true) => query.OrderByDescending(e => e.FirstName).ThenBy(e => e.Id),
            ("email", false) => query.OrderBy(e => e.Email).ThenBy(e => e.Id),
            ("email", true) => query.OrderByDescending(e => e.Email).ThenBy(e => e.Id),
            ("hiredate", false) => query.OrderBy(e => e.HireDate).ThenBy(e => e.Id),
            ("hiredate", true) => query.OrderByDescending(e => e.HireDate).ThenBy(e => e.Id),
            (_, true) => query.OrderByDescending(e => e.LastName).ThenBy(e => e.Id),
            _ => query.OrderBy(e => e.LastName).ThenBy(e => e.Id)
        };
    }

    public static IQueryable<Employee> ApplyPaging(this IQueryable<Employee> query, EmployeeQueryParameters parameters)
    {
        var page = Math.Max(parameters.Page, 1);

        return query.Skip((page - 1) * parameters.PageSize).Take(parameters.PageSize);
    }
}
