namespace EmployeeManagement.Application.Exceptions;

public class BusinessValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public BusinessValidationException(string field, string message)
        : base(message)
    {
        Errors = new Dictionary<string, string[]> { [field] = [message] };
    }
}
