using System.Reflection;
using EmployeeManagement.Api.Authorization;
using EmployeeManagement.Api.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace EmployeeManagement.Tests.Controllers;

/// <summary>Guards the authorization rules so a future refactor cannot silently open an endpoint.</summary>
public class AuthorizationRulesTests
{
    [Theory]
    [InlineData(typeof(EmployeesController))]
    [InlineData(typeof(DepartmentsController))]
    public void Controller_RequiresAuthenticationByDefault(Type controllerType)
    {
        Assert.NotNull(controllerType.GetCustomAttribute<AuthorizeAttribute>());
    }

    [Theory]
    [InlineData(typeof(EmployeesController), "CreateEmployee")]
    [InlineData(typeof(EmployeesController), "UpdateEmployee")]
    [InlineData(typeof(EmployeesController), "UpdateEmployeeStatus")]
    [InlineData(typeof(EmployeesController), "DeleteEmployee")]
    [InlineData(typeof(DepartmentsController), "CreateDepartment")]
    [InlineData(typeof(DepartmentsController), "UpdateDepartment")]
    [InlineData(typeof(DepartmentsController), "DeleteDepartment")]
    public void WriteAction_RequiresAdminPolicy(Type controllerType, string actionName)
    {
        var action = controllerType.GetMethod(actionName);

        var attribute = action?.GetCustomAttribute<AuthorizeAttribute>();

        Assert.NotNull(attribute);
        Assert.Equal(AuthorizationPolicies.AdminOnly, attribute.Policy);
    }

    [Theory]
    [InlineData(typeof(EmployeesController), "GetEmployees")]
    [InlineData(typeof(EmployeesController), "GetEmployeeById")]
    [InlineData(typeof(DepartmentsController), "GetDepartments")]
    [InlineData(typeof(DepartmentsController), "GetDepartmentSummaries")]
    public void ReadAction_DoesNotRequireAdminPolicy(Type controllerType, string actionName)
    {
        var action = controllerType.GetMethod(actionName);

        Assert.Null(action?.GetCustomAttribute<AuthorizeAttribute>());
    }

    [Fact]
    public void LoginAction_AllowsAnonymousCallers()
    {
        var action = typeof(AuthController).GetMethod(nameof(AuthController.Login));

        Assert.NotNull(action?.GetCustomAttribute<AllowAnonymousAttribute>());
    }
}
