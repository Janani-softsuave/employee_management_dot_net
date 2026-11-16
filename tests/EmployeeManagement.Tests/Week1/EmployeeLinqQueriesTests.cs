using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Exercises.Linq;
using EmployeeManagement.Tests.Builders;

namespace EmployeeManagement.Tests.Week1;

public class EmployeeLinqQueriesTests
{
    private static List<Employee> Sample() =>
    [
        TestData.Employee(id: 1, firstName: "Asha", lastName: "Nair", email: "asha@x.com", departmentId: 1, salary: 100m, hireDate: new DateOnly(2022, 3, 1)),
        TestData.Employee(id: 2, firstName: "Ravi", lastName: "Kumar", email: "ravi@x.com", departmentId: 1, salary: 200m, hireDate: new DateOnly(2023, 7, 1), isActive: false),
        TestData.Employee(id: 3, firstName: "Meera", lastName: "Iyer", email: "meera@x.com", departmentId: 2, salary: 300m, hireDate: new DateOnly(2021, 1, 1)),
        TestData.Employee(id: 4, firstName: "Karan", lastName: "Shah", email: "karan@x.com", departmentId: 2, salary: 500m, hireDate: new DateOnly(2024, 9, 9))
    ];

    [Fact]
    public void GetActive_ReturnsOnlyActiveEmployees()
    {
        var result = EmployeeLinqQueries.GetActive(Sample()).Select(e => e.Id).ToList();

        Assert.Equal([1, 3, 4], result);
    }

    [Fact]
    public void GetByDepartmentAndMinSalary_AppliesBothConditions()
    {
        var result = EmployeeLinqQueries.GetByDepartmentAndMinSalary(Sample(), departmentId: 2, minSalary: 400m);

        Assert.Equal(4, Assert.Single(result).Id);
    }

    [Fact]
    public void GetByDepartmentAndMinSalary_SalaryEqualToMinimum_IsIncluded()
    {
        var result = EmployeeLinqQueries.GetByDepartmentAndMinSalary(Sample(), departmentId: 1, minSalary: 200m);

        Assert.Equal(2, Assert.Single(result).Id);
    }

    [Fact]
    public void GetFullNamesSorted_ReturnsNamesAlphabetically()
    {
        var result = EmployeeLinqQueries.GetFullNamesSorted(Sample()).ToList();

        Assert.Equal(["Asha Nair", "Karan Shah", "Meera Iyer", "Ravi Kumar"], result);
    }

    [Fact]
    public void GetNewestHires_ReturnsRequestedCountNewestFirst()
    {
        var result = EmployeeLinqQueries.GetNewestHires(Sample(), 2).Select(e => e.Id).ToList();

        Assert.Equal([4, 2], result);
    }

    [Fact]
    public void GetNewestHires_CountLargerThanSource_ReturnsEverything()
    {
        var result = EmployeeLinqQueries.GetNewestHires(Sample(), 100).ToList();

        Assert.Equal(4, result.Count);
    }

    [Fact]
    public void CountByDepartment_GroupsHeadCount()
    {
        var result = EmployeeLinqQueries.CountByDepartment(Sample());

        Assert.Equal(2, result[1]);
        Assert.Equal(2, result[2]);
    }

    [Fact]
    public void CountByDepartment_EmptySource_ReturnsEmptyDictionary()
    {
        Assert.Empty(EmployeeLinqQueries.CountByDepartment([]));
    }

    [Fact]
    public void AverageSalaryByDepartment_ComputesAveragePerGroup()
    {
        var result = EmployeeLinqQueries.AverageSalaryByDepartment(Sample());

        Assert.Equal(150m, result[1]);
        Assert.Equal(400m, result[2]);
    }

    [Fact]
    public void HighestPaidPerDepartment_ReturnsOneEmployeePerDepartment()
    {
        var result = EmployeeLinqQueries.HighestPaidPerDepartment(Sample()).Select(e => e.Id).ToList();

        Assert.Equal([2, 4], result);
    }

    [Theory]
    [InlineData("KAR", 1)]
    [InlineData("x.com", 4)]
    [InlineData("nobody", 0)]
    public void SearchByText_MatchesNameOrEmailIgnoringCase(string text, int expectedCount)
    {
        var result = EmployeeLinqQueries.SearchByText(Sample(), text).ToList();

        Assert.Equal(expectedCount, result.Count);
    }

    [Fact]
    public void GetPage_ReturnsSecondPage()
    {
        var result = EmployeeLinqQueries.GetPage(Sample(), page: 2, pageSize: 3).Select(e => e.Id).ToList();

        Assert.Equal([4], result);
    }

    [Fact]
    public void GetPage_PageBeyondEnd_ReturnsEmpty()
    {
        Assert.Empty(EmployeeLinqQueries.GetPage(Sample(), page: 9, pageSize: 3));
    }

    [Fact]
    public void HasDuplicateEmails_SameEmailDifferentCase_ReturnsTrue()
    {
        var employees = Sample();
        employees.Add(TestData.Employee(id: 5, email: "ASHA@X.COM"));

        Assert.True(EmployeeLinqQueries.HasDuplicateEmails(employees));
    }

    [Fact]
    public void HasDuplicateEmails_AllUnique_ReturnsFalse()
    {
        Assert.False(EmployeeLinqQueries.HasDuplicateEmails(Sample()));
    }

    [Fact]
    public void BuildEmailIndex_AllowsCaseInsensitiveLookup()
    {
        var index = EmployeeLinqQueries.BuildEmailIndex(Sample());

        Assert.Equal(3, index["MEERA@x.com"].Id);
    }

    [Fact]
    public void Filter_UsesTheDelegateSuppliedByTheCaller()
    {
        var result = EmployeeLinqQueries.Filter(Sample(), e => e.Salary > 250m).Select(e => e.Id).ToList();

        Assert.Equal([3, 4], result);
    }

    [Fact]
    public void DeferredExecution_QueryRunsWhenEnumeratedNotWhenDefined()
    {
        // Arrange
        var employees = Sample();
        var activeQuery = EmployeeLinqQueries.GetActive(employees); // nothing has run yet

        // Act: change the source AFTER the query was defined
        employees.Add(TestData.Employee(id: 99, email: "late@x.com"));
        var ids = activeQuery.Select(e => e.Id).ToList();

        // Assert: the late addition is seen because the query executes now
        Assert.Contains(99, ids);
    }

    [Theory]
    [InlineData(2026, 6, 15, 2024, 6, 15, SeniorityLevel.Mid)]      // exactly 2 years
    [InlineData(2026, 6, 14, 2024, 6, 15, SeniorityLevel.Junior)]   // one day short of 2 years
    [InlineData(2026, 6, 15, 2026, 6, 15, SeniorityLevel.Junior)]   // hired today
    [InlineData(2026, 6, 15, 2021, 6, 15, SeniorityLevel.Senior)]   // exactly 5 years
    [InlineData(2026, 6, 15, 2022, 1, 1, SeniorityLevel.Mid)]
    public void ClassifySeniority_UsesCompletedYearsOfService(
        int todayYear, int todayMonth, int todayDay, int hireYear, int hireMonth, int hireDay, SeniorityLevel expected)
    {
        var employee = TestData.Employee(hireDate: new DateOnly(hireYear, hireMonth, hireDay));

        var result = EmployeeLinqQueries.ClassifySeniority(employee, new DateOnly(todayYear, todayMonth, todayDay));

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ClassifySeniority_HireDateInFuture_Throws()
    {
        var employee = TestData.Employee(hireDate: new DateOnly(2030, 1, 1));

        Assert.Throws<ArgumentOutOfRangeException>(
            () => EmployeeLinqQueries.ClassifySeniority(employee, new DateOnly(2026, 6, 15)));
    }
}
