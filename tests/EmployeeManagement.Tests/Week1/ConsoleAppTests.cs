using EmployeeManagement.ConsoleApp.Models;
using EmployeeManagement.ConsoleApp.Services;

namespace EmployeeManagement.Tests.Week1;

public class ConsoleAppTests
{
    private static List<EmployeeRecord> Sample() =>
    [
        new() { Id = 1, Name = "Asha Nair", Department = "Engineering", Salary = 100m },
        new() { Id = 2, Name = "Ravi Kumar", Department = "Engineering", Salary = 200m, ManagerId = 1 },
        new() { Id = 3, Name = "Meera Iyer", Department = "Finance", Salary = 300m }
    ];

    [Fact]
    public void TryParse_ValidLineWithManager_ReturnsRecord()
    {
        var ok = EmployeeParser.TryParse("2,Ravi Kumar,Engineering,72000.50,1", out var record, out var error);

        Assert.True(ok);
        Assert.Null(error);
        Assert.Equal("Ravi Kumar", record!.Name);
        Assert.Equal(72000.50m, record.Salary);
        Assert.Equal(1, record.ManagerId);
    }

    [Fact]
    public void TryParse_EmptyManagerField_LeavesManagerIdNull()
    {
        EmployeeParser.TryParse("1,Asha Nair,Engineering,85000,", out var record, out _);

        Assert.Null(record!.ManagerId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("1,Only,Three")]
    [InlineData("abc,Name,Dept,100,")]
    [InlineData("0,Name,Dept,100,")]
    [InlineData("1,,Dept,100,")]
    [InlineData("1,Name,Dept,-5,")]
    [InlineData("1,Name,Dept,notanumber,")]
    [InlineData("1,Name,Dept,100,x")]
    public void TryParse_InvalidLine_ReturnsFalseWithError(string? line)
    {
        var ok = EmployeeParser.TryParse(line, out var record, out var error);

        Assert.False(ok);
        Assert.Null(record);
        Assert.False(string.IsNullOrEmpty(error));
    }

    [Fact]
    public void ParseAll_MixedLines_ReturnsGoodRecordsAndCollectsErrorsWithLineNumbers()
    {
        var errors = new List<string>();

        var records = EmployeeParser.ParseAll(["1,A,Dept,10,", "bad line", "3,C,Dept,30,"], errors);

        Assert.Equal(2, records.Count);
        Assert.Equal("Line 2: Expected 5 comma-separated fields but found 1.", Assert.Single(errors));
    }

    [Fact]
    public void Search_MatchesNameOrDepartmentIgnoringCase()
    {
        var byName = EmployeeReport.Search(Sample(), "ravi");
        var byDepartment = EmployeeReport.Search(Sample(), "FINANCE");

        Assert.Equal(2, Assert.Single(byName).Id);
        Assert.Equal(3, Assert.Single(byDepartment).Id);
    }

    [Fact]
    public void BuildStatistics_ReportsTotalsAndDepartmentCounts()
    {
        var text = EmployeeReport.BuildStatistics(Sample());

        Assert.Contains("Employees:      3", text);
        Assert.Contains("Total payroll:  600.00", text);
        Assert.Contains("Average salary: 200.00", text);
        Assert.Contains("Highest paid:   Meera Iyer (300.00)", text);
        Assert.Contains("Engineering: 2", text);
        Assert.Contains("Finance: 1", text);
    }

    [Fact]
    public void BuildStatistics_NoEmployees_ReturnsFriendlyMessage()
    {
        Assert.Equal("No employees to summarise.", EmployeeReport.BuildStatistics([]));
    }

    [Fact]
    public void BuildListing_ShowsDashWhenThereIsNoManager()
    {
        var text = EmployeeReport.BuildListing(Sample());

        Assert.Contains("Asha Nair", text);
        Assert.Contains("-", text.Split('\n')[1]);
    }

    [Fact]
    public void Struct_IsCopiedOnAssignment_SoOriginalIsUnaffected()
    {
        var original = new SalaryRange(10m, 20m);
        var copy = original with { Max = 99m };

        Assert.Equal(20m, original.Max);
        Assert.Equal(99m, copy.Max);
    }

    [Fact]
    public void Class_IsSharedByReference_SoBothVariablesSeeTheSameObject()
    {
        var first = Sample()[0];
        var second = first;

        Assert.Same(first, second);
    }

    [Theory]
    [InlineData(100, 100, 200, true)]
    [InlineData(200, 100, 200, true)]
    [InlineData(99, 100, 200, false)]
    [InlineData(201, 100, 200, false)]
    public void SalaryRange_Contains_IncludesBothBoundaries(int salary, int min, int max, bool expected)
    {
        Assert.Equal(expected, new SalaryRange(min, max).Contains(salary));
    }

    [Fact]
    public void FilterBySalaryRange_ReturnsOnlyEmployeesInsideTheRange()
    {
        var result = EmployeeReport.FilterBySalaryRange(Sample(), new SalaryRange(150m, 350m));

        Assert.Equal([2, 3], result.Select(e => e.Id).ToArray());
    }

    [Fact]
    public async Task ExportAsync_WritesHeaderAndOneLinePerEmployee()
    {
        var path = Path.Combine(Path.GetTempPath(), $"employees-{Guid.NewGuid():N}.csv");

        try
        {
            await EmployeeExporter.ExportAsync(Sample(), path);

            var lines = await File.ReadAllLinesAsync(path);
            Assert.Equal(4, lines.Length);
            Assert.Equal("Id,Name,Department,Salary,ManagerId", lines[0]);
            Assert.Equal("2,Ravi Kumar,Engineering,200,1", lines[2]);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task ExportAsync_CancelledToken_ThrowsOperationCanceledException()
    {
        var path = Path.Combine(Path.GetTempPath(), $"employees-{Guid.NewGuid():N}.csv");
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        try
        {
            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => EmployeeExporter.ExportAsync(Sample(), path, cts.Token));
        }
        finally
        {
            File.Delete(path);
        }
    }
}
