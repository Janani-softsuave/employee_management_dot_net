# LINQ exercise file (Week 1, Day 3)

Code: `src/EmployeeManagement.Exercises/Linq/EmployeeLinqQueries.cs`
Tests: `tests/EmployeeManagement.Tests/Week1/EmployeeLinqQueriesTests.cs`

Each query is a small pure method over `IEnumerable<Employee>`, so it can be unit-tested without a database.
`n` = number of employees.

## Sample input used for every query

| Id | Name | Dept | Salary | Hired | Active | Email |
|----|------|------|--------|-------|--------|-------|
| 1 | Asha Nair | 1 | 100 | 2022-03-01 | yes | asha@x.com |
| 2 | Ravi Kumar | 1 | 200 | 2023-07-01 | no | ravi@x.com |
| 3 | Meera Iyer | 2 | 300 | 2021-01-01 | yes | meera@x.com |
| 4 | Karan Shah | 2 | 500 | 2024-09-09 | yes | karan@x.com |

## Queries

| # | Method and LINQ operators | Input | Output | Complexity |
|---|---------------------------|-------|--------|------------|
| 1 | `GetActive` — `Where` | sample | employees 1, 3, 4 | O(n) |
| 2 | `GetByDepartmentAndMinSalary` — `Where` with `&&` | dept 2, min 400 | employee 4 | O(n) |
| 3 | `GetFullNamesSorted` — `Select`, `OrderBy` | sample | Asha Nair, Karan Shah, Meera Iyer, Ravi Kumar | O(n log n) |
| 4 | `GetNewestHires` — `OrderByDescending`, `ThenBy`, `Take` | count 2 | employees 4, 2 | O(n log n) |
| 5 | `CountByDepartment` — `GroupBy`, `ToDictionary` | sample | `{1: 2, 2: 2}` | O(n) |
| 6 | `AverageSalaryByDepartment` — `GroupBy`, `Average` | sample | `{1: 150, 2: 400}` | O(n) |
| 7 | `HighestPaidPerDepartment` — `GroupBy`, `MaxBy`, `OrderBy` | sample | employees 2, 4 | O(n) plus O(g log g) |
| 8 | `SearchByText` — `Where`, `Contains` | `"KAR"` | employee 4 (Karan Shah) | O(n · m), m = text length |
| 9 | `GetPage` — `OrderBy`, `Skip`, `Take` | page 2, size 3 | employee 4 | O(n log n) |
| 10 | `HasDuplicateEmails` — `GroupBy`, `Any` | sample plus `ASHA@X.COM` | `true` | O(n) |
| 11 | `BuildEmailIndex` — `ToDictionary` | sample | dictionary; lookup `"MEERA@x.com"` returns employee 3 | O(n) to build, O(1) per lookup |
| 12 | `Filter<T>` — delegate `Func<T,bool>` | `e => e.Salary > 250` | employees 3, 4 | O(n) |
| 13 | `ClassifySeniority` — `switch` expression with relational patterns | hired 2024-06-15, today 2026-06-15 | `Mid` | O(1) |

Also in this exercise set:
* **Events:** `EmployeeRegistry.EmployeeAdded` is raised whenever `Add` succeeds (`EmployeeRegistryTests`).
* **Deferred execution:** `DeferredExecution_QueryRunsWhenEnumeratedNotWhenDefined` proves a query sees items added to the
  source after the query was written, because LINQ executes when you enumerate (`ToList`, `foreach`), not when you define the query.
* **Lambdas and delegates:** query 12 receives its filter as a `Func<T,bool>`.
* **Generics:** `Filter<T>` works for any element type.
* **Extension methods:** the repository's `ApplyFilters`/`ApplySorting`/`ApplyPaging` in
  `Infrastructure/Persistence/Extensions` are extension methods over `IQueryable<Employee>`.

## Notes on complexity

* `Where`, `Select`, `Take`, `Skip` walk the sequence once: O(n).
* Any sort (`OrderBy`) is O(n log n).
* `GroupBy` builds a hash table of groups: O(n) on average.
* A `Dictionary` lookup is O(1) on average; a `List` search is O(n). This is why query 11 builds an index first.
* `IEnumerable<T>` queries run in memory in C#. On `IQueryable<T>` (EF Core) the same expression is translated to SQL and runs in the database.
