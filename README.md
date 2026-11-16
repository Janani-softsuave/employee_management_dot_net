# Employee Management API

An ASP.NET Core Web API for **Employee & Department Management**, built as the Week 1 (C# foundations)
and Week 2 (Web API) capstone of the .NET training programme.

It uses only the concepts from those two weeks: C# OOP, records, generics, LINQ, async/await with
cancellation, xUnit; and controllers, DTOs, dependency injection, middleware, layered architecture,
EF Core with migrations, validation, central error handling and JWT authentication/authorization.

## Solution structure

```
EmployeeManagement.slnx
├── src
│   ├── EmployeeManagement.Domain          Entities only. No dependencies.
│   ├── EmployeeManagement.Application     DTOs, interfaces, services (business rules), mappings, exceptions
│   ├── EmployeeManagement.Infrastructure  EF Core DbContext, configurations, repositories, JWT token service
│   └── EmployeeManagement.Api             Controllers, middleware, authorization policies, Program.cs
└── tests
    └── EmployeeManagement.Tests           xUnit tests (Arrange-Act-Assert)
```

### Layer dependencies

```
        Api  ─────────────┐
         │                │ (composition root only: registers services)
         ▼                ▼
   Application  ◄───  Infrastructure
         │
         ▼
      Domain
```

* **Domain** knows nothing about other layers.
* **Application** defines *interfaces* (`IEmployeeRepository`, `ITokenService`, ...) and the business rules.
* **Infrastructure** *implements* those interfaces (EF Core, JWT). Dependencies point inwards.
* **Api** handles HTTP concerns only and delegates to services.

### Responsibilities

| Piece | Responsibility |
|-------|----------------|
| Controller | HTTP: routing, model binding, status codes. No business logic. |
| Service | Business rules (unique email, hire date, department must exist). |
| Repository | Data access through EF Core. Hides queries behind an interface. |
| DTO | The API contract. Entities are never exposed directly. |
| Middleware | Cross-cutting HTTP concerns: error handling and request logging. |

## Request pipeline

Order matters: each middleware wraps everything registered after it.

```
Request
  → ExceptionHandlingMiddleware   catches any exception, returns ProblemDetails
  → RequestLoggingMiddleware      logs method, path, status, duration (no bodies or headers)
  → HttpsRedirection
  → Authentication                reads the JWT, builds the user (401 if token invalid)
  → Authorization                 checks roles/policies (403 if not allowed)
  → Controller action → Service → Repository → Database
Response travels back out through the same middleware.
```

## Service lifetimes

| Lifetime | Registered | Why |
|----------|-----------|-----|
| Singleton | `IDateTimeProvider`, `IUserStore` | Stateless, one instance for the app. |
| Scoped | `AppDbContext`, repositories, `IUnitOfWork`, services | One per HTTP request; share one DbContext. |
| Transient | `ITokenService` | Cheap, new instance each time. |

A **captive dependency** is a longer-lived service holding a shorter-lived one (e.g. a singleton
injecting a scoped `DbContext`). That is why services that use repositories are scoped, never singleton.

## Getting started

Prerequisite: .NET SDK 10.0.x (`global.json` pins 10.0.403 and allows newer feature bands).

```bash
# 1. Restore packages and local tools (dotnet-ef)
dotnet restore
dotnet tool restore

# 2. Store the JWT signing key outside source control (user-secrets). Use at least 32 characters.
dotnet user-secrets set "Jwt:Key" "<your-own-long-random-secret>" --project src/EmployeeManagement.Api

# 3. Create the first migration (only needed once; see "Database")
dotnet ef migrations add InitialCreate \
  --project src/EmployeeManagement.Infrastructure \
  --startup-project src/EmployeeManagement.Api \
  --output-dir Persistence/Migrations

# 4. Build, test and run
dotnet build
dotnet test
dotnet run --project src/EmployeeManagement.Api --launch-profile https
```

Then open **https://localhost:7273/swagger** (OpenAPI document: `/openapi/v1.json`).

To call protected endpoints from Swagger UI: run `POST /api/auth/login`, copy the `accessToken`, click **Authorize**, paste the token and confirm.
Ready-made requests, covering success and failure cases, are in
`src/EmployeeManagement.Api/EmployeeManagement.Api.http`.

In production, supply the key as an environment variable instead: `Jwt__Key=<secret>`.

## Database

* EF Core with **SQLite** (file `employeemanagement.db`, created next to the API). Connection string: `ConnectionStrings:DefaultConnection`.
* In the Development environment the app runs `Database.Migrate()` on start-up, creating the database and
  seeding 3 departments and 4 employees.
* Relationship: one `Department` has many `Employee` (`DeleteBehavior.Restrict` prevents orphaning).
* Reading queries use `AsNoTracking()`. `Include()` (eager loading) fetches the department together with
  employees in one SQL query, avoiding N+1.
* To see the generated SQL, run in Development: the `Microsoft.EntityFrameworkCore.Database.Command`
  log category is set to `Information` in `appsettings.Development.json`.
* Other commands:
  ```bash
  dotnet ef migrations add <Name> --project src/EmployeeManagement.Infrastructure --startup-project src/EmployeeManagement.Api --output-dir Persistence/Migrations
  dotnet ef database update      --project src/EmployeeManagement.Infrastructure --startup-project src/EmployeeManagement.Api
  ```

## API summary and status codes

All endpoints except `POST /api/auth/login` require a `Bearer` token.

| Method | Route | Role | Success | Failures |
|--------|-------|------|---------|----------|
| POST | `/api/auth/login` | anonymous | 200 | 400, 401 |
| GET | `/api/employees` (filter, sort, page) | any user | 200 | 401 |
| GET | `/api/employees/{id}` | any user | 200 | 401, 404 |
| POST | `/api/employees` | Admin | 201 + Location | 400, 401, 403, 409 |
| PUT | `/api/employees/{id}` | Admin | 204 | 400, 401, 403, 404, 409 |
| PATCH | `/api/employees/{id}/status` | Admin | 204 | 400, 401, 403, 404 |
| DELETE | `/api/employees/{id}` | Admin | 204 | 401, 403, 404 |
| GET | `/api/departments` | any user | 200 | 401 |
| GET | `/api/departments/{id}` | any user | 200 | 401, 404 |
| GET | `/api/departments/summary` | any user | 200 | 401 |
| POST | `/api/departments` | Admin | 201 + Location | 400, 401, 403, 409 |
| PUT | `/api/departments/{id}` | Admin | 204 | 400, 401, 403, 404, 409 |
| DELETE | `/api/departments/{id}` | Admin | 204 | 401, 403, 404, 409 (has employees) |

Employee list query string: `search`, `departmentId`, `isActive`, `sortBy` (`firstName`, `lastName`, `email`,
`hireDate`), `descending`, `page`, `pageSize` (max 50).

**PUT vs PATCH:** `PUT /employees/{id}` replaces the whole employee; `PATCH /employees/{id}/status` changes one field.
Both are idempotent here: repeating the same call leaves the same state.

**201 / 204 / 400 / 404:** 201 = created (with a `Location` header), 204 = success with no body,
400 = the request is malformed or breaks a rule, 404 = the resource does not exist.

### Error responses

Every error is an RFC 7807 `ProblemDetails` JSON body with a `traceId`:

```json
{
  "title": "Resource not found",
  "status": 404,
  "detail": "Employee with id '9999' was not found.",
  "instance": "/api/employees/9999",
  "traceId": "0HN..."
}
```

Validation failures return a `errors` dictionary keyed by field. Unexpected errors return a generic
500 message; full details are only written to the log.

## Authentication vs authorization, 401 vs 403

* **Authentication** answers *who are you?* The client logs in and receives a signed **JWT**.
* **Authorization** answers *what may you do?* Rules use the token's **claims** (a claim is a fact about
  the user, e.g. `role = Admin`). Here the `AdminOnly` policy requires the `Admin` role.
* **401 Unauthorized**: no token, or an invalid/expired one. The server does not know who you are.
* **403 Forbidden**: the token is valid, but your role is not allowed to perform the action.

Demo users (Development only, `appsettings.Development.json`): `admin` / `Admin@12345` (role Admin) and
`user` / `User@12345` (role User). These are throwaway development credentials, not secrets for a real system.
A real deployment should store hashed passwords in a database.

## Configuration and secrets

| Setting | Where |
|---------|-------|
| `ConnectionStrings:DefaultConnection`, `Jwt:Issuer`, `Jwt:Audience`, `Jwt:ExpiryMinutes` | `appsettings.json` |
| Logging levels, demo users | `appsettings.Development.json` (environment-specific) |
| `Jwt:Key` | **Never committed.** user-secrets (dev) or `Jwt__Key` environment variable (other environments). |

The app refuses to start if `Jwt:Key` is missing or shorter than 32 characters.

## Where each training concept is used

| Concept | Where |
|---------|-------|
| Classes, properties, inheritance, navigation properties | `Domain/Entities` (`BaseEntity`) |
| Interfaces, composition, SOLID (dependency inversion) | `Application/Interfaces`, constructor injection everywhere |
| Records | All DTOs |
| Generics | `IRepository<T>`, `Repository<T>`, `PagedResult<T>` |
| Extension methods, lambdas, pattern matching | `Mappings/*`, `EmployeeQueryableExtensions` (switch expression) |
| LINQ Where/Select/GroupBy/OrderBy, deferred execution | `EmployeeQueryableExtensions`, `DepartmentService.GetSummariesAsync` |
| async/await + `CancellationToken` | Every service, repository and controller action |
| Exceptions | `Application/Exceptions`, `ExceptionHandlingMiddleware` |
| xUnit, Arrange-Act-Assert, edge cases, regression test | `tests/EmployeeManagement.Tests` |
| Options pattern | `JwtSettings`, `DemoUserSettings` |
| Logging with `ILogger`, scopes, structured properties | Services, `RequestLoggingMiddleware` |

## Tests

```bash
dotnet test
```

Tests use hand-written in-memory fakes (no database), a fixed clock for deterministic dates, and cover
normal, boundary and invalid inputs, middleware status mapping, JWT contents, and the authorization
attributes on every endpoint. `CreateAsync_DuplicateEmailWithDifferentCase_ThrowsConflictException`
is the regression test for the case-sensitive duplicate-email defect.

## Known limitations

* SQLite is used for easy local setup; swap `UseSqlite` in `Infrastructure/DependencyInjection.cs` for another provider if needed.
* Demo users come from configuration, with no registration, password hashing or refresh tokens.
* Salary sorting is not offered because SQLite cannot `ORDER BY` a `decimal` column.
* There is no optimistic concurrency token; the last write wins.
* The build/test evidence (`dotnet build`, `dotnet test` output) should be captured on the machine you submit from.
