using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Exercises.Linq;

/// <summary>
/// Day 3 LINQ exercise file. Every query is a small pure method over <see cref="IEnumerable{T}"/>,
/// so each one is unit-testable. Input, output and complexity for each query are documented in
/// docs/Week1/LinqQueries.md. n = number of employees, g = number of groups.
/// </summary>
public static class EmployeeLinqQueries
{
    /// <summary>Q1 Where. O(n). Deferred: nothing runs until the result is enumerated.</summary>
    public static IEnumerable<Employee> GetActive(IEnumerable<Employee> employees) =>
        employees.Where(e => e.IsActive);

    /// <summary>Q2 Where with two conditions. O(n).</summary>
    public static IEnumerable<Employee> GetByDepartmentAndMinSalary(
        IEnumerable<Employee> employees, int departmentId, decimal minSalary) =>
        employees.Where(e => e.DepartmentId == departmentId && e.Salary >= minSalary);

    /// <summary>Q3 Select + OrderBy: projects to a different type. O(n log n) for the sort.</summary>
    public static IEnumerable<string> GetFullNamesSorted(IEnumerable<Employee> employees) =>
        employees
            .Select(e => e.FullName)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase);

    /// <summary>Q4 OrderByDescending + Take: the most recent hires. O(n log n).</summary>
    public static IEnumerable<Employee> GetNewestHires(IEnumerable<Employee> employees, int count) =>
        employees
            .OrderByDescending(e => e.HireDate)
            .ThenBy(e => e.Id)
            .Take(count);

    /// <summary>Q5 GroupBy + ToDictionary: head count per department. O(n).</summary>
    public static Dictionary<int, int> CountByDepartment(IEnumerable<Employee> employees) =>
        employees
            .GroupBy(e => e.DepartmentId)
            .ToDictionary(group => group.Key, group => group.Count());

    /// <summary>Q6 GroupBy + Average: average salary per department. O(n).</summary>
    public static Dictionary<int, decimal> AverageSalaryByDepartment(IEnumerable<Employee> employees) =>
        employees
            .GroupBy(e => e.DepartmentId)
            .ToDictionary(group => group.Key, group => group.Average(e => e.Salary));

    /// <summary>Q7 GroupBy + MaxBy: the highest paid employee in each department. O(n).</summary>
    public static IEnumerable<Employee> HighestPaidPerDepartment(IEnumerable<Employee> employees) =>
        employees
            .GroupBy(e => e.DepartmentId)
            .Select(group => group.MaxBy(e => e.Salary)!)
            .OrderBy(e => e.DepartmentId);

    /// <summary>Q8 Where + Contains: case-insensitive search of names and email. O(n * length of text).</summary>
    public static IEnumerable<Employee> SearchByText(IEnumerable<Employee> employees, string text) =>
        employees.Where(e =>
            e.FullName.Contains(text, StringComparison.OrdinalIgnoreCase) ||
            e.Email.Contains(text, StringComparison.OrdinalIgnoreCase));

    /// <summary>Q9 OrderBy + Skip + Take: pagination. O(n log n) for the sort, then O(page size).</summary>
    public static IEnumerable<Employee> GetPage(IEnumerable<Employee> employees, int page, int pageSize) =>
        employees
            .OrderBy(e => e.Id)
            .Skip((Math.Max(page, 1) - 1) * pageSize)
            .Take(pageSize);

    /// <summary>Q10 GroupBy + Any: detects duplicate emails, ignoring case. O(n).</summary>
    public static bool HasDuplicateEmails(IEnumerable<Employee> employees) =>
        employees
            .GroupBy(e => e.Email, StringComparer.OrdinalIgnoreCase)
            .Any(group => group.Count() > 1);

    /// <summary>
    /// Q11 ToDictionary: builds an index for fast lookup. O(n) once, then each lookup is O(1).
    /// Compare with a List, where every lookup is O(n).
    /// </summary>
    public static Dictionary<string, Employee> BuildEmailIndex(IEnumerable<Employee> employees) =>
        employees.ToDictionary(e => e.Email, StringComparer.OrdinalIgnoreCase);

    /// <summary>Q12 Delegate parameter: the caller decides the filter. O(n).</summary>
    public static IEnumerable<T> Filter<T>(IEnumerable<T> source, Func<T, bool> predicate) =>
        source.Where(predicate);

    /// <summary>
    /// Q13 Pattern matching (switch expression with relational patterns) to classify years of service.
    /// O(1).
    /// </summary>
    public static SeniorityLevel ClassifySeniority(Employee employee, DateOnly today)
    {
        int years = today.Year - employee.HireDate.Year;

        if (employee.HireDate.AddYears(years) > today)
        {
            years--;
        }

        return years switch
        {
            < 0 => throw new ArgumentOutOfRangeException(nameof(employee), "Hire date is in the future."),
            < 2 => SeniorityLevel.Junior,
            < 5 => SeniorityLevel.Mid,
            _ => SeniorityLevel.Senior
        };
    }
}
