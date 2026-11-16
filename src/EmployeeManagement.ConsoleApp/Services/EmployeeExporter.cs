using System.Globalization;
using EmployeeManagement.ConsoleApp.Models;

namespace EmployeeManagement.ConsoleApp.Services;

public static class EmployeeExporter
{

    public static async Task ExportAsync(
        IReadOnlyList<EmployeeRecord> employees,
        string path,
        CancellationToken cancellationToken = default)
    {
        await using var writer = new StreamWriter(path, append: false);

        await writer.WriteLineAsync("Id,Name,Department,Salary,ManagerId".AsMemory(), cancellationToken);

        foreach (EmployeeRecord employee in employees)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string managerId = employee.ManagerId?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
            string line = string.Join(',',
                employee.Id.ToString(CultureInfo.InvariantCulture),
                employee.Name,
                employee.Department,
                employee.Salary.ToString(CultureInfo.InvariantCulture),
                managerId);

            await writer.WriteLineAsync(line.AsMemory(), cancellationToken);
        }
    }
}
