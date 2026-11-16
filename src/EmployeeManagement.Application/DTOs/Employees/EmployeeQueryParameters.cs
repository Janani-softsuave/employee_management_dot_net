namespace EmployeeManagement.Application.DTOs.Employees;

/// <summary>Filtering, sorting and pagination options bound from the query string.</summary>
public record EmployeeQueryParameters
{
    private const int MaxPageSize = 50;

    private readonly int _pageSize = 10;

    public string? Search { get; init; }

    public int? DepartmentId { get; init; }

    public bool? IsActive { get; init; }

    /// <summary>One of: firstName, lastName, email, hireDate. Defaults to lastName.</summary>
    public string? SortBy { get; init; }

    public bool Descending { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = Math.Clamp(value, 1, MaxPageSize);
    }
}
