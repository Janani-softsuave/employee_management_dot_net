using System.Globalization;
using EmployeeManagement.ConsoleApp.Models;

namespace EmployeeManagement.ConsoleApp.Services;

public static class EmployeeExporter
{
    /// <summary>
    /// Writes employees to a CSV file asynchronously.
    /// Cancellation: the token is checked before every line, so a cancel request stops the export
    /// promptly and surfaces as an <see cref="OperationCanceledException"/>.
    /// </summary>
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
