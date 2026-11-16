using EmployeeManagement.Application.DTOs.Departments;
using EmployeeManagement.Application.Exceptions;
using EmployeeManagement.Application.Services;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Tests.Builders;
using EmployeeManagement.Tests.Fakes;
using Microsoft.Extensions.Logging.Abstractions;

namespace EmployeeManagement.Tests.Services;

public class DepartmentServiceTests
{
    private readonly FakeDepartmentRepository _departments = new();
    private readonly FakeEmployeeRepository _employees;
    private readonly DepartmentService _sut;

    public DepartmentServiceTests()
    {
        _employees = new FakeEmployeeRepository { Departments = _departments };
        _departments.EmployeeRepository = _employees;

        _sut = new DepartmentService(
            _departments,
            _employees,
            new FakeUnitOfWork(),
            NullLogger<DepartmentService>.Instance);
    }

    [Fact]
    public async Task CreateAsync_NewName_ReturnsDepartmentWithGeneratedId()
    {

        var result = await _sut.CreateAsync(new CreateDepartmentDto { Name = "  Legal ", Description = "Contracts" });

        Assert.Equal("Legal", result.Name);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task CreateAsync_DuplicateNameIgnoringCase_ThrowsConflictException()
    {

        await _sut.CreateAsync(new CreateDepartmentDto { Name = "Legal" });

        await Assert.ThrowsAsync<ConflictException>(
            () => _sut.CreateAsync(new CreateDepartmentDto { Name = "LEGAL" }));
    }

    [Fact]
    public async Task DeleteAsync_DepartmentHasEmployees_ThrowsConflictException()
    {

        _departments.Departments.Add(new Department { Id = 1, Name = "Engineering" });
        _employees.Employees.Add(TestData.Employee(id: 1, departmentId: 1));

        await Assert.ThrowsAsync<ConflictException>(() => _sut.DeleteAsync(1));
        Assert.Single(_departments.Departments);
    }

    [Fact]
    public async Task DeleteAsync_EmptyDepartment_RemovesIt()
    {

        _departments.Departments.Add(new Department { Id = 1, Name = "Engineering" });

        await _sut.DeleteAsync(1);

        Assert.Empty(_departments.Departments);
    }

    [Fact]
    public async Task GetByIdAsync_UnknownId_ThrowsNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetByIdAsync(404));
    }

    [Fact]
    public async Task GetSummariesAsync_GroupsActiveEmployeesByDepartmentWithAverageSalary()
    {

        _departments.Departments.Add(new Department { Id = 1, Name = "Engineering" });
        _departments.Departments.Add(new Department { Id = 2, Name = "Finance" });
        _employees.Employees.Add(TestData.Employee(id: 1, email: "a@x.com", departmentId: 1, salary: 100m));
        _employees.Employees.Add(TestData.Employee(id: 2, email: "b@x.com", departmentId: 1, salary: 200m));
        _employees.Employees.Add(TestData.Employee(id: 3, email: "c@x.com", departmentId: 2, salary: 300m));
        _employees.Employees.Add(TestData.Employee(id: 4, email: "d@x.com", departmentId: 2, salary: 900m, isActive: false));

        var summaries = await _sut.GetSummariesAsync();

        Assert.Collection(
            summaries,
            engineering =>
            {
                Assert.Equal("Engineering", engineering.DepartmentName);
                Assert.Equal(2, engineering.EmployeeCount);
                Assert.Equal(150m, engineering.AverageSalary);
            },
            finance =>
            {
                Assert.Equal("Finance", finance.DepartmentName);
                Assert.Equal(1, finance.EmployeeCount);
                Assert.Equal(300m, finance.AverageSalary);
            });
    }

    [Fact]
    public async Task GetSummariesAsync_NoEmployees_ReturnsEmptyList()
    {
        var summaries = await _sut.GetSummariesAsync();

        Assert.Empty(summaries);
    }
}
