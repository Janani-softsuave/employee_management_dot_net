namespace EmployeeManagement.Domain.Entities;

public class Department : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    // Navigation property: one department has many employees.
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
