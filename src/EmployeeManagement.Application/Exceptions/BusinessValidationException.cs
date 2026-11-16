namespace EmployeeManagement.Application.Exceptions;

/// <summary>Thrown when a business rule is violated (mapped to HTTP 400 with field errors).</summary>
public class BusinessValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public BusinessValidationException(string field, string message)
        : base(message)
    {
        Errors = new Dictionary<string, string[]> { [field] = [message] };
    }
}
