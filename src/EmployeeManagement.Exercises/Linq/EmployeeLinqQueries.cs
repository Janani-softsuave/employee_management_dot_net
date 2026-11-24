using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Exercises.Linq;

public static class EmployeeLinqQueries
{

    public static IEnumerable<Employee> GetActive(IEnumerable<Employee> employees) =>
        employees.Where(e => e.IsActive);

    public static IEnumerable<Employee> GetByDepartmentAndMinSalary(
        IEnumerable<Employee> employees, int departmentId, decimal minSalary) =>
        employees.Where(e => e.DepartmentId == departmentId && e.Salary >= minSalary);

    public static IEnumerable<string> GetFullNamesSorted(IEnumerable<Employee> employees) =>
        employees
            .Select(e => e.FullName)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase);

    public static IEnumerable<Employee> GetNewestHires(IEnumerable<Employee> employees, int count) =>
        employees
            .OrderByDescending(e => e.HireDate)
            .ThenBy(e => e.Id)
            .Take(count);

    public static Dictionary<int, int> CountByDepartment(IEnumerable<Employee> employees) =>
        employees
            .GroupBy(e => e.DepartmentId)
            .ToDictionary(group => group.Key, group => group.Count());

    public static Dictionary<int, decimal> AverageSalaryByDepartment(IEnumerable<Employee> employees) =>
        employees
            .GroupBy(e => e.DepartmentId)
            .ToDictionary(group => group.Key, group => group.Average(e => e.Salary));

    public static IEnumerable<Employee> HighestPaidPerDepartment(IEnumerable<Employee> employees) =>
        employees
            .GroupBy(e => e.DepartmentId)
            .Select(group => group.MaxBy(e => e.Salary)!)
            .OrderBy(e => e.DepartmentId);

    public static IEnumerable<Employee> SearchByText(IEnumerable<Employee> employees, string text) =>
        employees.Where(e =>
            e.FullName.Contains(text, StringComparison.OrdinalIgnoreCase) ||
            e.Email.Contains(text, StringComparison.OrdinalIgnoreCase));

    public static IEnumerable<Employee> GetPage(IEnumerable<Employee> employees, int page, int pageSize) =>
        employees
            .OrderBy(e => e.Id)
            .Skip((Math.Max(page, 1) - 1) * pageSize)
            .Take(pageSize);

    public static bool HasDuplicateEmails(IEnumerable<Employee> employees) =>
        employees
            .GroupBy(e => e.Email, StringComparer.OrdinalIgnoreCase)
            .Any(group => group.Count() > 1);

    public static Dictionary<string, Employee> BuildEmailIndex(IEnumerable<Employee> employees) =>
        employees.ToDictionary(e => e.Email, StringComparer.OrdinalIgnoreCase);

    public static IEnumerable<T> Filter<T>(IEnumerable<T> source, Func<T, bool> predicate) =>
        source.Where(predicate);

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
