namespace EmployeeManagement.Domain.Entities;

public class Employee : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateOnly HireDate { get; set; }

    public decimal Salary { get; set; }

    public bool IsActive { get; set; } = true;

    // Foreign key + navigation property: each employee belongs to one department.
    public int DepartmentId { get; set; }

    public Department? Department { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}
