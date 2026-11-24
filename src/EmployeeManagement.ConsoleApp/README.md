# Employee Console App (Week 1, Day 1)

A console application that processes employee records using methods, collections, strings and exception-safe parsing.
It deliberately uses plain loops, `List<T>` and `Dictionary<TKey, TValue>` (LINQ comes on Day 3).

## Run

Prerequisite: .NET SDK 10.0.x. From the solution folder:

```bash
dotnet run --project src/EmployeeManagement.ConsoleApp -- <command>
```

| Command | What it does |
|---------|--------------|
| `list` | Show all employees |
| `search <text>` | Employees whose name or department contains the text (case-insensitive) |
| `stats` | Total payroll, average, highest paid, head count per department |
| `parse "<Id,Name,Department,Salary,ManagerId>"` | Validate a single CSV line |
| `export <path>` | Write all employees to CSV asynchronously; Ctrl+C cancels cleanly |

Exit codes: `0` success, `1` bad usage, `2` invalid line, `3` file error, `130` cancelled.

## Three sample inputs and outputs

> The outputs below were derived from the code by calculation. Re-run the commands on your machine and
> paste the real output here before you submit.

**1. `dotnet run --project src/EmployeeManagement.ConsoleApp -- search ravi`**

```
Id  Name             Department           Salary  Manager
2   Ravi Kumar       Engineering       72,000.00  1
```

**2. `dotnet run --project src/EmployeeManagement.ConsoleApp -- stats`**

```
Employees:      5
Total payroll:  382,000.00
Average salary: 76,400.00
Highest paid:   Divya Rao (91,000.00)
By department:
  Engineering: 3
  Finance: 1
  Human Resources: 1
```

**3. `dotnet run --project src/EmployeeManagement.ConsoleApp -- parse "7,Test,Finance,abc,"`**

```
Invalid: Salary must be a positive number.
```

## Note: value types and reference types used

| Type | Kind | Where | Why it matters |
|------|------|-------|----------------|
| `int`, `decimal`, `bool` | Value types | `Id`, `Salary`, loop counters, totals | Stored directly in the variable; assignment copies the value. `decimal` is used for money because it avoids binary rounding errors. |
| `int?` (`Nullable<int>`) | Nullable value type | `EmployeeRecord.ManagerId` | `null` cleanly means "no manager" instead of a magic number such as 0 or -1. Checked with `HasValue`/`.Value`. |
| `SalaryRange` | `readonly record struct` (value type) | `Models/SalaryRange.cs` | Small, immutable and copied on assignment. `with` creates a modified copy and leaves the original untouched. |
| `EmployeeRecord` | `class` (reference type) | `Models/EmployeeRecord.cs` | Variables hold a reference to one heap object. `var b = a;` makes both point to the same object. |
| `string` | Reference type, immutable | Names, CSV lines | Every "change" creates a new string. `StringBuilder` is used when building reports so repeated appends do not allocate a new string each time. |
| `List<T>`, `Dictionary<,>`, `string[]` | Reference types | Collections | Passing them to a method passes the reference, so the method can modify the same collection. |
| `string?` / `EmployeeRecord?` | Nullable reference types | `TryParse` out parameters | `#nullable enable` makes the compiler warn about possible null dereferences. |

`ExportAsync` shows `async`/`await` with a `CancellationToken`: the token is checked before each line, and the
`Ctrl+C` handler cancels it so the file is closed properly rather than the process dying mid-write.
