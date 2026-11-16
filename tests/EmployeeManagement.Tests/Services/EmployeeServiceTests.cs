using EmployeeManagement.Application.DTOs.Employees;
using EmployeeManagement.Application.Exceptions;
using EmployeeManagement.Application.Services;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Tests.Builders;
using EmployeeManagement.Tests.Fakes;
using Microsoft.Extensions.Logging.Abstractions;

namespace EmployeeManagement.Tests.Services;

public class EmployeeServiceTests
{
    private readonly FakeDepartmentRepository _departments = new();
    private readonly FakeEmployeeRepository _employees;
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly EmployeeService _sut;

    public EmployeeServiceTests()
    {
        _departments.Departments.Add(new Department { Id = 1, Name = "Engineering" });
        _employees = new FakeEmployeeRepository { Departments = _departments };

        _sut = new EmployeeService(
            _employees,
            _departments,
            _unitOfWork,
            new FakeDateTimeProvider(),
            NullLogger<EmployeeService>.Instance);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_PersistsEmployeeAndReturnsDto()
    {

        var dto = TestData.CreateEmployeeDto(email: "New.Hire@Example.com");

        var result = await _sut.CreateAsync(dto);

        Assert.Equal("new.hire@example.com", result.Email);
        Assert.Equal("Engineering", result.DepartmentName);
        Assert.True(result.IsActive);
        Assert.Single(_employees.Employees);
        Assert.Equal(1, _unitOfWork.SaveCount);
    }

    [Fact]
    public async Task CreateAsync_DuplicateEmail_ThrowsConflictException()
    {

        _employees.Employees.Add(TestData.Employee(id: 10, email: "taken@example.com"));
        var dto = TestData.CreateEmployeeDto(email: "taken@example.com");

        await Assert.ThrowsAsync<ConflictException>(() => _sut.CreateAsync(dto));
        Assert.Equal(0, _unitOfWork.SaveCount);
    }

    [Fact]
    public async Task CreateAsync_DuplicateEmailWithDifferentCase_ThrowsConflictException()
    {

        _employees.Employees.Add(TestData.Employee(id: 10, email: "taken@example.com"));
        var dto = TestData.CreateEmployeeDto(email: "  TAKEN@Example.com ");

        await Assert.ThrowsAsync<ConflictException>(() => _sut.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_HireDateInFuture_ThrowsBusinessValidationException()
    {

        var dto = TestData.CreateEmployeeDto(hireDate: new DateOnly(2026, 6, 16));

        var exception = await Assert.ThrowsAsync<BusinessValidationException>(() => _sut.CreateAsync(dto));

        Assert.Contains("HireDate", exception.Errors.Keys);
    }

    [Fact]
    public async Task CreateAsync_HireDateIsToday_IsAccepted()
    {

        var dto = TestData.CreateEmployeeDto(hireDate: new DateOnly(2026, 6, 15));

        var result = await _sut.CreateAsync(dto);

        Assert.Equal(new DateOnly(2026, 6, 15), result.HireDate);
    }

    [Fact]
    public async Task CreateAsync_UnknownDepartment_ThrowsBusinessValidationException()
    {

        var dto = TestData.CreateEmployeeDto(departmentId: 999);

        var exception = await Assert.ThrowsAsync<BusinessValidationException>(() => _sut.CreateAsync(dto));

        Assert.Contains("DepartmentId", exception.Errors.Keys);
    }

    [Fact]
    public async Task GetByIdAsync_UnknownId_ThrowsNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetByIdAsync(42));
    }

    [Fact]
    public async Task UpdateAsync_ExistingEmployee_ReplacesAllFields()
    {

        _employees.Employees.Add(TestData.Employee(id: 5));
        var dto = TestData.UpdateEmployeeDto(email: "Updated@Example.com");

        await _sut.UpdateAsync(5, dto);

        var stored = _employees.Employees.Single();
        Assert.Equal("Updated", stored.FirstName);
        Assert.Equal("updated@example.com", stored.Email);
        Assert.Equal(60000m, stored.Salary);
    }

    [Fact]
    public async Task UpdateAsync_KeepingOwnEmail_DoesNotConflictWithItself()
    {

        _employees.Employees.Add(TestData.Employee(id: 5, email: "same@example.com"));
        var dto = TestData.UpdateEmployeeDto(email: "same@example.com");

        await _sut.UpdateAsync(5, dto);
    }

    [Fact]
    public async Task UpdateAsync_EmailBelongsToAnotherEmployee_ThrowsConflictException()
    {

        _employees.Employees.Add(TestData.Employee(id: 5, email: "five@example.com"));
        _employees.Employees.Add(TestData.Employee(id: 6, email: "six@example.com"));

        await Assert.ThrowsAsync<ConflictException>(
            () => _sut.UpdateAsync(5, TestData.UpdateEmployeeDto(email: "six@example.com")));
    }

    [Fact]
    public async Task UpdateAsync_UnknownEmployee_ThrowsNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.UpdateAsync(77, TestData.UpdateEmployeeDto()));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateStatusAsync_ExistingEmployee_SetsIsActive(bool isActive)
    {

        _employees.Employees.Add(TestData.Employee(id: 3, isActive: !isActive));

        await _sut.UpdateStatusAsync(3, new UpdateEmployeeStatusDto(isActive));

        Assert.Equal(isActive, _employees.Employees.Single().IsActive);
    }

    [Fact]
    public async Task DeleteAsync_ExistingEmployee_RemovesIt()
    {

        _employees.Employees.Add(TestData.Employee(id: 8));

        await _sut.DeleteAsync(8);

        Assert.Empty(_employees.Employees);
    }

    [Fact]
    public async Task DeleteAsync_UnknownEmployee_ThrowsNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.DeleteAsync(8));
    }

    [Fact]
    public async Task CreateAsync_TokenAlreadyCancelled_ThrowsOperationCanceledAndSavesNothing()
    {

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => _sut.CreateAsync(TestData.CreateEmployeeDto(), cts.Token));

        Assert.Empty(_employees.Employees);
        Assert.Equal(0, _unitOfWork.SaveCount);
    }

    [Fact]
    public async Task GetEmployeesAsync_ReturnsPagedResultWithRequestedPageSize()
    {

        _employees.Employees.Add(TestData.Employee(id: 1, email: "a@example.com"));
        _employees.Employees.Add(TestData.Employee(id: 2, email: "b@example.com"));

        var result = await _sut.GetEmployeesAsync(new EmployeeQueryParameters { Page = 1, PageSize = 5 });

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(1, result.TotalPages);
    }
}
