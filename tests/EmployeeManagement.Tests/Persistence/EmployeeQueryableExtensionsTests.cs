using EmployeeManagement.Application.DTOs.Employees;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Infrastructure.Persistence.Extensions;
using EmployeeManagement.Tests.Builders;

namespace EmployeeManagement.Tests.Persistence;

public class EmployeeQueryableExtensionsTests
{
    private static IQueryable<Employee> Sample() =>
        new List<Employee>
        {
            TestData.Employee(id: 1, firstName: "Asha", lastName: "Nair", email: "asha@x.com", departmentId: 1, hireDate: new DateOnly(2022, 3, 1)),
            TestData.Employee(id: 2, firstName: "Ravi", lastName: "Kumar", email: "ravi@x.com", departmentId: 1, hireDate: new DateOnly(2023, 7, 1), isActive: false),
            TestData.Employee(id: 3, firstName: "Meera", lastName: "Iyer", email: "meera@x.com", departmentId: 2, hireDate: new DateOnly(2021, 1, 1)),
        }.AsQueryable();

    [Fact]
    public void ApplyFilters_SearchMatchesNameOrEmailIgnoringCase()
    {
        var result = Sample().ApplyFilters(new EmployeeQueryParameters { Search = "KUM" }).ToList();

        Assert.Equal(2, Assert.Single(result).Id);
    }

    [Fact]
    public void ApplyFilters_ByDepartment_ReturnsOnlyThatDepartment()
    {
        var result = Sample().ApplyFilters(new EmployeeQueryParameters { DepartmentId = 1 }).ToList();

        Assert.Equal([1, 2], result.Select(e => e.Id).ToArray());
    }

    [Fact]
    public void ApplyFilters_ByActiveFlag_ReturnsOnlyActive()
    {
        var result = Sample().ApplyFilters(new EmployeeQueryParameters { IsActive = true }).ToList();

        Assert.DoesNotContain(result, e => !e.IsActive);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void ApplyFilters_NoCriteria_ReturnsEverything()
    {
        var result = Sample().ApplyFilters(new EmployeeQueryParameters()).ToList();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void ApplyFilters_NoMatch_ReturnsEmpty()
    {
        var result = Sample().ApplyFilters(new EmployeeQueryParameters { Search = "zzz" }).ToList();

        Assert.Empty(result);
    }

    [Fact]
    public void ApplySorting_DefaultsToLastNameAscending()
    {
        var result = Sample().ApplySorting(new EmployeeQueryParameters()).Select(e => e.LastName).ToList();

        Assert.Equal(["Iyer", "Kumar", "Nair"], result);
    }

    [Fact]
    public void ApplySorting_HireDateDescending_ReturnsNewestFirst()
    {
        var result = Sample()
            .ApplySorting(new EmployeeQueryParameters { SortBy = "hireDate", Descending = true })
            .Select(e => e.Id)
            .ToList();

        Assert.Equal([2, 1, 3], result);
    }

    [Fact]
    public void ApplySorting_UnknownColumn_FallsBackToLastName()
    {
        var result = Sample()
            .ApplySorting(new EmployeeQueryParameters { SortBy = "salary; DROP TABLE" })
            .Select(e => e.LastName)
            .ToList();

        Assert.Equal(["Iyer", "Kumar", "Nair"], result);
    }

    [Fact]
    public void ApplyPaging_SecondPage_SkipsFirstPage()
    {
        var result = Sample()
            .ApplySorting(new EmployeeQueryParameters())
            .ApplyPaging(new EmployeeQueryParameters { Page = 2, PageSize = 2 })
            .ToList();

        Assert.Equal(3, Assert.Single(result).Id);
    }

    [Fact]
    public void ApplyPaging_PageBelowOne_IsTreatedAsFirstPage()
    {
        var result = Sample()
            .ApplySorting(new EmployeeQueryParameters())
            .ApplyPaging(new EmployeeQueryParameters { Page = -4, PageSize = 2 })
            .ToList();

        Assert.Equal(2, result.Count);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-5, 1)]
    [InlineData(25, 25)]
    [InlineData(5000, 50)]
    public void PageSize_IsClampedBetweenOneAndFifty(int requested, int expected)
    {
        var parameters = new EmployeeQueryParameters { PageSize = requested };

        Assert.Equal(expected, parameters.PageSize);
    }
}
