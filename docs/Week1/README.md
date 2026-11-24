# Week 1 submission map

Where each item of the Week 1 checklist lives. Capstone domain: Employee & Department Management.

| Day | Checklist item | Location |
|-----|----------------|----------|
| 1 | Console application | `src/EmployeeManagement.ConsoleApp` |
| 1 | README with run instructions and 3 sample inputs/outputs | `src/EmployeeManagement.ConsoleApp/README.md` |
| 1 | Note on value and reference types | same README, section "Note: value types and reference types used" |
| 1 | LeetCode: Concatenation of Array | `Exercises/LeetCode/ConcatenationOfArray.cs` |
| 2 | Domain model class library with 3+ classes | `src/EmployeeManagement.Domain` (`BaseEntity`, `Employee`, `Department`) |
| 2 | Interface-based design with one implementation | `IEmployeeRepository` → `EmployeeRepository`, `IEmployeeService` → `EmployeeService` |
| 2 | Class diagram and SOLID explanation | `docs/Week1/ClassDiagramAndSolid.md` |
| 2 | LeetCode: Number of Steps to Reduce a Number to Zero | `Exercises/LeetCode/NumberOfSteps.cs` |
| 3 | LINQ exercise file with 8+ queries | `Exercises/Linq/EmployeeLinqQueries.cs` (13 queries) |
| 3 | Unit-testable filtering/grouping methods | same file, tested in `Tests/Week1/EmployeeLinqQueriesTests.cs` |
| 3 | Input, output and complexity per query | `docs/Week1/LinqQueries.md` |
| 3 | LeetCode: Contains Duplicate | `Exercises/LeetCode/ContainsDuplicateSolution.cs` |
| 4 | Solution with app, class library and xUnit projects | `EmployeeManagement.slnx` (ConsoleApp + Domain/Application/Infrastructure/Exercises + Tests) |
| 4 | 5+ passing tests using Arrange-Act-Assert | `tests/EmployeeManagement.Tests` (see "Evidence" below) |
| 4 | `dotnet build` / `dotnet test` log | See "Evidence" below |
| 4 | Async method with cancellation | `EmployeeExporter.ExportAsync`; also every service method accepts a `CancellationToken` |
| 4 | LeetCode: Best Time to Buy and Sell Stock | `Exercises/LeetCode/BestTimeToBuyAndSellStock.cs` |
| 5 | Mini-project and README | the whole solution; root `README.md` |
| 5 | Tests for normal, boundary, invalid inputs | `EmployeeServiceTests`, `ConsoleAppTests`, `LeetCodeTests` |
| 5 | Regression test | `CreateAsync_DuplicateEmailWithDifferentCase_ThrowsConflictException` |
| 5 | Walkthrough of a class, a LINQ query and an async operation | `docs/Week1/Walkthrough.md` |
| 5 | LeetCode: Valid Anagram | `Exercises/LeetCode/ValidAnagram.cs` |
| All | LeetCode log | `docs/Week1/LeetCodeLog.md` |

## Evidence (to complete on your machine)

```bash
dotnet build  > docs/Week1/build.log
dotnet test   > docs/Week1/test.log
```

Attach or screenshot `build.log` and `test.log` showing success. Interview-prep topics for each day are covered in the
class diagram/SOLID note, the LINQ notes (deferred execution, `IEnumerable` vs `IQueryable`, `List` vs `Dictionary`) and
the walkthrough (`Task` vs `Thread`, Arrange-Act-Assert, value vs reference types).
