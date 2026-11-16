namespace EmployeeManagement.Application.Exceptions;

/// <summary>Thrown when a requested resource does not exist (mapped to HTTP 404).</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string resourceName, object key)
        : base($"{resourceName} with id '{key}' was not found.")
    {
    }
}
