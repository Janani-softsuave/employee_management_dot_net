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

/// <summary>Day 3 events exercise: an in-memory registry that raises an event when an employee is added.</summary>
public class EmployeeRegistry
{
    private readonly List<Employee> _employees = new();

    /// <summary>Subscribers register with += and are notified every time Add succeeds.</summary>
    public event EventHandler<EmployeeAddedEventArgs>? EmployeeAdded;

    public IReadOnlyList<Employee> Employees => _employees;

    public void Add(Employee employee)
    {
        ArgumentNullException.ThrowIfNull(employee);

        _employees.Add(employee);
        EmployeeAdded?.Invoke(this, new EmployeeAddedEventArgs(employee));
    }
}
