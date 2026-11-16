namespace EmployeeManagement.Application.Interfaces;

/// <summary>Commits all pending repository changes as one transaction.</summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
