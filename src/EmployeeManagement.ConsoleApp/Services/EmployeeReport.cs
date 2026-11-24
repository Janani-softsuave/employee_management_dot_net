using System.Globalization;
using System.Text;
using EmployeeManagement.ConsoleApp.Models;

namespace EmployeeManagement.ConsoleApp.Services;

public static class EmployeeReport
{
    public static string BuildListing(IReadOnlyList<EmployeeRecord> employees)
    {

        var builder = new StringBuilder();
        builder.AppendLine(string.Format(
            CultureInfo.InvariantCulture, "{0,-3} {1,-16} {2,-16} {3,10}  {4}", "Id", "Name", "Department", "Salary", "Manager"));

        foreach (EmployeeRecord employee in employees)
        {
            string manager = employee.ManagerId.HasValue
                ? employee.ManagerId.Value.ToString(CultureInfo.InvariantCulture)
                : "-";

            builder.AppendLine(string.Format(
                CultureInfo.InvariantCulture,
                "{0,-3} {1,-16} {2,-16} {3,10:N2}  {4}",
                employee.Id,
                employee.Name,
                employee.Department,
                employee.Salary,
                manager));
        }

        return builder.ToString().TrimEnd();
    }

    public static List<EmployeeRecord> Search(IReadOnlyList<EmployeeRecord> employees, string term)
    {
        var matches = new List<EmployeeRecord>();

        foreach (EmployeeRecord employee in employees)
        {
            bool nameMatches = employee.Name.Contains(term, StringComparison.OrdinalIgnoreCase);
            bool departmentMatches = employee.Department.Contains(term, StringComparison.OrdinalIgnoreCase);

            if (nameMatches || departmentMatches)
            {
                matches.Add(employee);
            }
        }

        return matches;
    }

    public static string BuildStatistics(IReadOnlyList<EmployeeRecord> employees)
    {
        if (employees.Count == 0)
        {
            return "No employees to summarise.";
        }

        decimal total = 0m;
        EmployeeRecord highest = employees[0];
        var countByDepartment = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (EmployeeRecord employee in employees)
        {
            total += employee.Salary;

            if (employee.Salary > highest.Salary)
            {
                highest = employee;
            }

            countByDepartment.TryGetValue(employee.Department, out int currentCount);
            countByDepartment[employee.Department] = currentCount + 1;
        }

        decimal average = total / employees.Count;

        var builder = new StringBuilder();
        builder.AppendLine($"Employees:      {employees.Count}");
        builder.AppendLine($"Total payroll:  {total.ToString("N2", CultureInfo.InvariantCulture)}");
        builder.AppendLine($"Average salary: {average.ToString("N2", CultureInfo.InvariantCulture)}");
        builder.AppendLine($"Highest paid:   {highest.Name} ({highest.Salary.ToString("N2", CultureInfo.InvariantCulture)})");
        builder.AppendLine("By department:");

        var departments = new List<string>(countByDepartment.Keys);
        departments.Sort(StringComparer.OrdinalIgnoreCase);

        foreach (string department in departments)
        {
            builder.AppendLine($"  {department}: {countByDepartment[department]}");
        }

        return builder.ToString().TrimEnd();
    }

    public static List<EmployeeRecord> FilterBySalaryRange(IReadOnlyList<EmployeeRecord> employees, SalaryRange range)
    {
        var result = new List<EmployeeRecord>();

        foreach (EmployeeRecord employee in employees)
        {
            if (range.Contains(employee.Salary))
            {
                result.Add(employee);
            }
        }

        return result;
    }
}
