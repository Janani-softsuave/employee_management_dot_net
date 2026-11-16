using EmployeeManagement.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Middleware;

/// <summary>
/// Central exception handling: converts exceptions into consistent RFC 7807 ProblemDetails responses.
/// Registered first in the pipeline so it can catch exceptions from everything after it.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // The client disconnected; there is nobody to respond to.
            _logger.LogInformation("Request {Method} {Path} was cancelled by the client", context.Request.Method, context.Request.Path);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var problem = exception switch
        {
            NotFoundException => CreateProblem(StatusCodes.Status404NotFound, "Resource not found", exception.Message),
            ConflictException => CreateProblem(StatusCodes.Status409Conflict, "Conflict", exception.Message),
            UnauthorizedAccessException => CreateProblem(StatusCodes.Status401Unauthorized, "Unauthorized", exception.Message),
            BusinessValidationException validation => new ValidationProblemDetails(validation.Errors)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "One or more validation errors occurred."
            },
            _ => CreateProblem(StatusCodes.Status500InternalServerError, "Server error", "An unexpected error occurred.")
        };

        if (problem.Status == StatusCodes.Status500InternalServerError)
        {
            // Full details go to the log only; the client gets a generic message.
            _logger.LogError(exception, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
        }
        else
        {
            _logger.LogWarning("Handled {ExceptionType}: {Message}", exception.GetType().Name, exception.Message);
        }

        problem.Instance = context.Request.Path;
        problem.Extensions["traceId"] = context.TraceIdentifier;

        context.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(problem, problem.GetType(), options: null, contentType: "application/problem+json");
    }

    private static ProblemDetails CreateProblem(int status, string title, string detail) =>
        new() { Status = status, Title = title, Detail = detail };
}
