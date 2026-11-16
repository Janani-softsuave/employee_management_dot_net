using EmployeeManagement.Application.DTOs.Departments;
using EmployeeManagement.Application.Exceptions;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Application.Mappings;
using EmployeeManagement.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DepartmentService> _logger;

    public DepartmentService(
        IDepartmentRepository departmentRepository,
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork,
        ILogger<DepartmentService> logger)
    {
        _departmentRepository = departmentRepository;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IReadOnlyList<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var departments = await _departmentRepository.GetAllAsync(cancellationToken);

        return departments.Select(d => d.ToDto()).ToList();
    }

    public async Task<DepartmentDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var department = await _departmentRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Department), id);

        return department.ToDto();
    }

    public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto, CancellationToken cancellationToken = default)
    {
        await EnsureNameIsUniqueAsync(dto.Name, null, cancellationToken);

        var department = dto.ToEntity();

        await _departmentRepository.AddAsync(department, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created department {DepartmentId}", department.Id);

        return department.ToDto();
    }

    public async Task UpdateAsync(int id, UpdateDepartmentDto dto, CancellationToken cancellationToken = default)
    {
        var department = await _departmentRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Department), id);

        await EnsureNameIsUniqueAsync(dto.Name, id, cancellationToken);

        department.Name = dto.Name.Trim();
        department.Description = dto.Description?.Trim();

        _departmentRepository.Update(department);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated department {DepartmentId}", id);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var department = await _departmentRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Department), id);

        if (await _departmentRepository.HasEmployeesAsync(id, cancellationToken))
        {
            throw new ConflictException("Department cannot be deleted while it still has employees.");
        }

        _departmentRepository.Remove(department);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted department {DepartmentId}", id);
    }

    /// <summary>Groups employees by department with a LINQ GroupBy/Select (done in memory: SQLite cannot average decimals).</summary>
    public async Task<IReadOnlyList<DepartmentSummaryDto>> GetSummariesAsync(CancellationToken cancellationToken = default)
    {
        var employees = await _employeeRepository.GetAllWithDepartmentAsync(cancellationToken);

        return employees
            .Where(e => e.IsActive)
            .GroupBy(e => new { e.DepartmentId, DepartmentName = e.Department?.Name ?? "Unknown" })
            .Select(g => new DepartmentSummaryDto(
                g.Key.DepartmentId,
                g.Key.DepartmentName,
                g.Count(),
                Math.Round(g.Average(e => e.Salary), 2)))
            .OrderBy(s => s.DepartmentName)
            .ToList();
    }

    private async Task EnsureNameIsUniqueAsync(string name, int? excludeDepartmentId, CancellationToken cancellationToken)
    {
        if (await _departmentRepository.NameExistsAsync(name.Trim(), excludeDepartmentId, cancellationToken))
        {
            throw new ConflictException($"A department named '{name.Trim()}' already exists.");
        }
    }
}
