namespace EmployeeManagement.Application.DTOs.Employees;

public record EmployeeQueryParameters
{
    private const int MaxPageSize = 50;

    private readonly int _pageSize = 10;

    public string? Search { get; init; }

    public int? DepartmentId { get; init; }

    public bool? IsActive { get; init; }

    public string? SortBy { get; init; }

    public bool Descending { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = Math.Clamp(value, 1, MaxPageSize);
    }
}
