namespace EmployeeManagement.Domain.Entities;

/// <summary>Common base class: every entity has an integer key.</summary>
public abstract class BaseEntity
{
    public int Id { get; set; }
}
