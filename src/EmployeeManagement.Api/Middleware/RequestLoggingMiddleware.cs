using System.Diagnostics;

namespace EmployeeManagement.Api.Middleware;

/// <summary>
/// Logs request/response metadata only (method, path, status code, duration).
/// Bodies, headers and query strings are deliberately not logged because they may contain secrets or personal data.
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Logger scope: every log line written while handling this request carries the same TraceId.
        using var scope = _logger.BeginScope(new Dictionary<string, object> { ["TraceId"] = context.TraceIdentifier });

        var stopwatch = Stopwatch.StartNew();

        await _next(context);

        stopwatch.Stop();

        _logger.LogInformation(
            "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds} ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds);
    }
}
