# Code walkthrough (Week 1, Day 5 / Gate 1)

Three short walkthroughs: one class, one LINQ query and one async operation.

## 1. One class: `EmployeeService`

`src/EmployeeManagement.Application/Services/EmployeeService.cs`

* **Purpose:** holds the business rules for employees: the email must be unique, the hire date cannot be in the future
  and the department must exist.
* **Design:** it receives `IEmployeeRepository`, `IDepartmentRepository`, `IUnitOfWork`, `IDateTimeProvider` and
  `ILogger` through its constructor (dependency inversion), so it never touches EF Core directly.
* **Encapsulation:** the rule checks are private methods (`ValidateHireDate`, `EnsureEmailIsUniqueAsync`), and the
  rest of the app only sees the `IEmployeeService` interface.
* **Errors:** rule violations throw typed exceptions (`ConflictException`, `BusinessValidationException`,
  `NotFoundException`). The API's exception middleware turns each into the right HTTP status, so the service stays
  free of HTTP concerns.
* **Why it is testable:** every dependency is an interface, so `EmployeeServiceTests` pass in-memory fakes and a fixed
  clock. Tests cover normal input, a boundary (hire date equal to today), invalid input and a regression.

## 2. One LINQ query: `HighestPaidPerDepartment`

`src/EmployeeManagement.Exercises/Linq/EmployeeLinqQueries.cs`

```csharp
employees
    .GroupBy(e => e.DepartmentId)
    .Select(group => group.MaxBy(e => e.Salary)!)
    .OrderBy(e => e.DepartmentId);
```

1. `GroupBy` splits the employees into one group per department.
2. `Select` turns each group into a single employee, the one with the biggest salary (`MaxBy`).
3. `OrderBy` makes the output order predictable.

* **Deferred execution:** the query only describes the work; it runs when the caller enumerates it (`ToList`, `foreach`).
* **Complexity:** one pass to group, one pass per group to find the maximum: O(n), plus O(g log g) to sort g groups.
* **Pitfall:** because it is deferred, changing the source list after defining the query changes the result. The test
  `DeferredExecution_QueryRunsWhenEnumeratedNotWhenDefined` demonstrates this.

## 3. One async operation: `EmployeeExporter.ExportAsync`

`src/EmployeeManagement.ConsoleApp/Services/EmployeeExporter.cs`

* It writes a CSV file with `await writer.WriteLineAsync(..., cancellationToken)`. While the disk is busy, the thread is
  released instead of blocked. It does not create a new thread: `async`/`await` is about not waiting idly, whereas a
  `Thread` is a unit of execution.
* **Cancellation:** `cancellationToken.ThrowIfCancellationRequested()` runs before each line. If the caller cancels (the
  console app wires `Ctrl+C` to a `CancellationTokenSource`), an `OperationCanceledException` is thrown, `await using`
  closes the file and `Program.cs` reports "Export cancelled." instead of crashing.
* **Pitfalls avoided:** the method returns `Task` rather than `async void`, so errors can be awaited and caught; there is
  no `.Result` or `.Wait()` that could deadlock; and the token is passed on to every async call.
* **Tested by:** `ExportAsync_WritesHeaderAndOneLinePerEmployee` and `ExportAsync_CancelledToken_ThrowsOperationCanceledException`.

## Regression test (injected-defect workflow)

Process used: reproduce the bug with a failing test, fix it, keep the test.

* **Defect:** the duplicate-email check compared emails case-sensitively, so `TAKEN@example.com` was accepted when
  `taken@example.com` already existed.
* **Fix:** emails are trimmed and lower-cased (`Trim().ToLowerInvariant()`) in `EmployeeService` before the check
  and when storing.
* **Regression test:** `EmployeeServiceTests.CreateAsync_DuplicateEmailWithDifferentCase_ThrowsConflictException`.

## Choosing test cases

For each rule I test the normal case, the boundary and the invalid input. Example for hire date: a past date is accepted,
today is accepted (boundary), tomorrow is rejected (invalid).

## One SOLID principle in my own code

**Dependency inversion:** `EmployeeService` depends on `IEmployeeRepository`, not on `EmployeeRepository`. That is
why the same service runs against an in-memory fake in tests and against SQLite in production.
