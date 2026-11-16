using EmployeeManagement.ConsoleApp.Models;
using EmployeeManagement.ConsoleApp.Services;

// Day 1 console application: processes employee records using methods and collections.
//
// Usage:
//   list                     show all employees
//   search <text>            find employees whose name or department contains the text
//   stats                    payroll statistics and head count per department
//   parse "<csv line>"       validate one line: Id,Name,Department,Salary,ManagerId
//   export <path>            write all employees to a CSV file (async, Ctrl+C cancels)

string[] sampleLines =
[
    "1,Asha Nair,Engineering,85000,",
    "2,Ravi Kumar,Engineering,72000,1",
    "3,Meera Iyer,Human Resources,64000,",
    "4,Karan Shah,Finance,70000,",
    "5,Divya Rao,Engineering,91000,1"
];

var parseErrors = new List<string>();
List<EmployeeRecord> employees = EmployeeParser.ParseAll(sampleLines, parseErrors);

if (args.Length == 0)
{
    PrintUsage();
    return 1;
}

try
{
    switch (args[0].ToLowerInvariant())
    {
        case "list":
            Console.WriteLine(EmployeeReport.BuildListing(employees));
            return 0;

        case "search" when args.Length >= 2:
            List<EmployeeRecord> matches = EmployeeReport.Search(employees, string.Join(' ', args[1..]));
            Console.WriteLine(matches.Count == 0 ? "No matches." : EmployeeReport.BuildListing(matches));
            return 0;

        case "stats":
            Console.WriteLine(EmployeeReport.BuildStatistics(employees));
            return 0;

        case "parse" when args.Length >= 2:
            if (EmployeeParser.TryParse(args[1], out EmployeeRecord? parsed, out string? error))
            {
                Console.WriteLine($"OK: {parsed!.Name} in {parsed.Department}, salary {parsed.Salary}");
                return 0;
            }

            Console.WriteLine($"Invalid: {error}");
            return 2;

        case "export" when args.Length >= 2:
            using (var cts = new CancellationTokenSource())
            {
                // Ctrl+C requests cancellation instead of killing the process mid-write.
                Console.CancelKeyPress += (_, eventArgs) =>
                {
                    eventArgs.Cancel = true;
                    cts.Cancel();
                };

                await EmployeeExporter.ExportAsync(employees, args[1], cts.Token);
                Console.WriteLine($"Exported {employees.Count} employees to {args[1]}");
            }

            return 0;

        default:
            PrintUsage();
            return 1;
    }
}
catch (OperationCanceledException)
{
    Console.WriteLine("Export cancelled.");
    return 130;
}
catch (IOException exception)
{
    Console.WriteLine($"File error: {exception.Message}");
    return 3;
}

static void PrintUsage()
{
    Console.WriteLine("Usage: list | search <text> | stats | parse \"<Id,Name,Department,Salary,ManagerId>\" | export <path>");
}
