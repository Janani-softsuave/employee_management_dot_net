using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Exercises.Linq;

public class EmployeeAddedEventArgs : EventArgs
{
    public EmployeeAddedEventArgs(Employee employee)
    {
        Employee = employee;
    }

    public Employee Employee { get; }
}

public class EmployeeRegistry
{
    private readonly List<Employee> _employees = new();

    public event EventHandler<EmployeeAddedEventArgs>? EmployeeAdded;

    public IReadOnlyList<Employee> Employees => _employees;

    public void Add(Employee employee)
    {
        ArgumentNullException.ThrowIfNull(employee);

        _employees.Add(employee);
        EmployeeAdded?.Invoke(this, new EmployeeAddedEventArgs(employee));
    }
}
