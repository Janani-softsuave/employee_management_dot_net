using EmployeeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeManagement.Infrastructure.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
        builder.Property(e => e.LastName).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Email).IsRequired().HasMaxLength(150);
        builder.Property(e => e.Salary).HasPrecision(18, 2);

        builder.Ignore(e => e.FullName);

        builder.HasIndex(e => e.Email).IsUnique();

        builder.HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new Employee { Id = 1, FirstName = "Asha", LastName = "Nair", Email = "asha.nair@example.com", HireDate = new DateOnly(2022, 3, 14), Salary = 85000m, IsActive = true, DepartmentId = 1 },
            new Employee { Id = 2, FirstName = "Ravi", LastName = "Kumar", Email = "ravi.kumar@example.com", HireDate = new DateOnly(2023, 7, 1), Salary = 72000m, IsActive = true, DepartmentId = 1 },
            new Employee { Id = 3, FirstName = "Meera", LastName = "Iyer", Email = "meera.iyer@example.com", HireDate = new DateOnly(2021, 11, 22), Salary = 64000m, IsActive = true, DepartmentId = 2 },
            new Employee { Id = 4, FirstName = "Karan", LastName = "Shah", Email = "karan.shah@example.com", HireDate = new DateOnly(2020, 1, 6), Salary = 70000m, IsActive = false, DepartmentId = 3 });
    }
}
