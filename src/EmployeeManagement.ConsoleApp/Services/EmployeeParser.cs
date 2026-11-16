using System.Globalization;
using EmployeeManagement.ConsoleApp.Models;

namespace EmployeeManagement.ConsoleApp.Services;

/// <summary>Turns text lines such as "1,Asha Nair,Engineering,85000,3" into records.</summary>
public static class EmployeeParser
{
    private const int ExpectedFieldCount = 5;

    /// <summary>
    /// Try-pattern: returns false and an error message instead of throwing for bad input.
    /// <paramref name="record"/> is a reference type, so it is null when parsing fails.
    /// </summary>
    public static bool TryParse(string? line, out EmployeeRecord? record, out string? error)
    {
        record = null;
        error = null;

        if (string.IsNullOrWhiteSpace(line))
        {
            error = "Line is empty.";
            return false;
        }

        string[] fields = line.Split(',');

        if (fields.Length != ExpectedFieldCount)
        {
            error = $"Expected {ExpectedFieldCount} comma-separated fields but found {fields.Length}.";
            return false;
        }

        if (!int.TryParse(fields[0].Trim(), out int id) || id <= 0)
        {
            error = "Id must be a positive whole number.";
            return false;
        }

        string name = fields[1].Trim();
        string department = fields[2].Trim();

        if (name.Length == 0 || department.Length == 0)
        {
            error = "Name and department are required.";
            return false;
        }

        if (!decimal.TryParse(fields[3].Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal salary) || salary <= 0)
        {
            error = "Salary must be a positive number.";
            return false;
        }

        int? managerId = null;
        string managerText = fields[4].Trim();

        if (managerText.Length > 0)
        {
            if (!int.TryParse(managerText, out int parsedManagerId))
            {
                error = "ManagerId must be a whole number or empty.";
                return false;
            }

            managerId = parsedManagerId;
        }

        record = new EmployeeRecord
        {
            Id = id,
            Name = name,
            Department = department,
            Salary = salary,
            ManagerId = managerId
        };

        return true;
    }

    /// <summary>Parses many lines. Bad lines are collected as errors; good lines are returned.</summary>
    public static List<EmployeeRecord> ParseAll(IEnumerable<string> lines, List<string> errors)
    {
        var records = new List<EmployeeRecord>();
        int lineNumber = 0;

        foreach (string line in lines)
        {
            lineNumber++;

            if (TryParse(line, out EmployeeRecord? record, out string? error))
            {
                records.Add(record!);
            }
            else
            {
                errors.Add($"Line {lineNumber}: {error}");
            }
        }

        return records;
    }
}
