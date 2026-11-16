using EmployeeManagement.Api.Controllers;
using EmployeeManagement.Application.DTOs.Employees;
using EmployeeManagement.Application.Services;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Tests.Builders;
using EmployeeManagement.Tests.Fakes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace EmployeeManagement.Tests.Controllers;

public class EmployeesControllerTests
{
    private readonly FakeEmployeeRepository _employees;
    private readonly EmployeesController _sut;

    public EmployeesControllerTests()
    {
        var departments = new FakeDepartmentRepository();
        departments.Departments.Add(new Department { Id = 1, Name = "Engineering" });
        _employees = new FakeEmployeeRepository { Departments = departments };

        var service = new EmployeeService(
            _employees, departments, new FakeUnitOfWork(), new FakeDateTimeProvider(), NullLogger<EmployeeService>.Instance);

        _sut = new EmployeesController(service);
    }

    [Fact]
    public async Task CreateEmployee_ValidRequest_Returns201WithLocationRoute()
    {

        var result = await _sut.CreateEmployee(TestData.CreateEmployeeDto(), CancellationToken.None);

        var created = Assert.IsType<CreatedAtRouteResult>(result.Result);
        Assert.Equal("GetEmployeeById", created.RouteName);
        var body = Assert.IsType<EmployeeDto>(created.Value);
        Assert.Equal(body.Id, created.RouteValues!["id"]);
    }

    [Fact]
    public async Task GetEmployeeById_ExistingEmployee_Returns200()
    {

        _employees.Employees.Add(TestData.Employee(id: 4));

        var result = await _sut.GetEmployeeById(4, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(4, Assert.IsType<EmployeeDto>(ok.Value).Id);
    }

    [Fact]
    public async Task UpdateEmployee_ExistingEmployee_Returns204()
    {

        _employees.Employees.Add(TestData.Employee(id: 4));

        var result = await _sut.UpdateEmployee(4, TestData.UpdateEmployeeDto(), CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateEmployeeStatus_ExistingEmployee_Returns204()
    {

        _employees.Employees.Add(TestData.Employee(id: 4));

        var result = await _sut.UpdateEmployeeStatus(4, new UpdateEmployeeStatusDto(false), CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteEmployee_ExistingEmployee_Returns204()
    {

        _employees.Employees.Add(TestData.Employee(id: 4));

        var result = await _sut.DeleteEmployee(4, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
    }
}
