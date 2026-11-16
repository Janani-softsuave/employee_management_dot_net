using EmployeeManagement.Exercises.Linq;
using EmployeeManagement.Tests.Builders;

namespace EmployeeManagement.Tests.Week1;

public class EmployeeRegistryTests
{
    [Fact]
    public void Add_RaisesEmployeeAddedEventWithTheEmployee()
    {
        // Arrange
        var registry = new EmployeeRegistry();
        EmployeeAddedEventArgs? received = null;
        registry.EmployeeAdded += (_, args) => received = args;
        var employee = TestData.Employee(id: 1);

        // Act
        registry.Add(employee);

        // Assert
        Assert.NotNull(received);
        Assert.Same(employee, received.Employee);
        Assert.Single(registry.Employees);
    }

    [Fact]
    public void Add_WithNoSubscribers_DoesNotThrow()
    {
        var registry = new EmployeeRegistry();

        registry.Add(TestData.Employee(id: 1));

        Assert.Single(registry.Employees);
    }

    [Fact]
    public void Add_NullEmployee_ThrowsArgumentNullException()
    {
        var registry = new EmployeeRegistry();

        Assert.Throws<ArgumentNullException>(() => registry.Add(null!));
    }
}
