using EmployeeManagement.Application.DTOs.Common;
using EmployeeManagement.Application.DTOs.Employees;
using EmployeeManagement.Application.Exceptions;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Application.Mappings;
using EmployeeManagement.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider,
        ILogger<EmployeeService> logger)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task<PagedResult<EmployeeDto>> GetEmployeesAsync(
        EmployeeQueryParameters query, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _employeeRepository.GetPagedAsync(query, cancellationToken);

        var dtos = items.Select(e => e.ToDto()).ToList();

        return new PagedResult<EmployeeDto>(dtos, totalCount, Math.Max(query.Page, 1), query.PageSize);
    }

    public async Task<EmployeeDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdWithDepartmentAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), id);

        return employee.ToDto();
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto, CancellationToken cancellationToken = default)
    {
        ValidateHireDate(dto.HireDate);
        await EnsureDepartmentExistsAsync(dto.DepartmentId, cancellationToken);
        await EnsureEmailIsUniqueAsync(dto.Email, null, cancellationToken);

        var employee = dto.ToEntity();

        await _employeeRepository.AddAsync(employee, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created employee {EmployeeId} in department {DepartmentId}", employee.Id, employee.DepartmentId);

        // Reload so the response includes the department name.
        return await GetByIdAsync(employee.Id, cancellationToken);
    }

    public async Task UpdateAsync(int id, UpdateEmployeeDto dto, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), id);

        ValidateHireDate(dto.HireDate);
        await EnsureDepartmentExistsAsync(dto.DepartmentId, cancellationToken);
        await EnsureEmailIsUniqueAsync(dto.Email, id, cancellationToken);

        dto.ApplyTo(employee);

        _employeeRepository.Update(employee);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated employee {EmployeeId}", id);
    }

    public async Task UpdateStatusAsync(int id, UpdateEmployeeStatusDto dto, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), id);

        employee.IsActive = dto.IsActive;

        _employeeRepository.Update(employee);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Employee {EmployeeId} active status set to {IsActive}", id, dto.IsActive);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), id);

        _employeeRepository.Remove(employee);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted employee {EmployeeId}", id);
    }

    private void ValidateHireDate(DateOnly hireDate)
    {
        if (hireDate > _dateTimeProvider.Today)
        {
            throw new BusinessValidationException(nameof(CreateEmployeeDto.HireDate), "Hire date cannot be in the future.");
        }
    }

    private async Task EnsureDepartmentExistsAsync(int departmentId, CancellationToken cancellationToken)
    {
        var department = await _departmentRepository.GetByIdAsync(departmentId, cancellationToken);

        if (department is null)
        {
            throw new BusinessValidationException(nameof(CreateEmployeeDto.DepartmentId), $"Department '{departmentId}' does not exist.");
        }
    }

    private async Task EnsureEmailIsUniqueAsync(string email, int? excludeEmployeeId, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        if (await _employeeRepository.EmailExistsAsync(normalizedEmail, excludeEmployeeId, cancellationToken))
        {
            throw new ConflictException($"An employee with email '{normalizedEmail}' already exists.");
        }
    }
}
