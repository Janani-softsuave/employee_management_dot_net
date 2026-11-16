using System.Text.Json;
using EmployeeManagement.Api.Middleware;
using EmployeeManagement.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace EmployeeManagement.Tests.Middleware;

public class ExceptionHandlingMiddlewareTests
{
    private static async Task<(int Status, JsonElement Body)> RunAsync(Exception exceptionToThrow)
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var sut = new ExceptionHandlingMiddleware(
            _ => throw exceptionToThrow,
            NullLogger<ExceptionHandlingMiddleware>.Instance);

        await sut.InvokeAsync(context);

        context.Response.Body.Position = 0;
        var body = await JsonDocument.ParseAsync(context.Response.Body);

        return (context.Response.StatusCode, body.RootElement.Clone());
    }

    [Fact]
    public async Task InvokeAsync_NotFoundException_Returns404ProblemDetails()
    {
        var (status, body) = await RunAsync(new NotFoundException("Employee", 7));

        Assert.Equal(StatusCodes.Status404NotFound, status);
        Assert.Equal(404, body.GetProperty("status").GetInt32());
        Assert.Contains("7", body.GetProperty("detail").GetString());
    }

    [Fact]
    public async Task InvokeAsync_ConflictException_Returns409()
    {
        var (status, _) = await RunAsync(new ConflictException("duplicate"));

        Assert.Equal(StatusCodes.Status409Conflict, status);
    }

    [Fact]
    public async Task InvokeAsync_UnauthorizedAccessException_Returns401()
    {
        var (status, _) = await RunAsync(new UnauthorizedAccessException("bad login"));

        Assert.Equal(StatusCodes.Status401Unauthorized, status);
    }

    [Fact]
    public async Task InvokeAsync_BusinessValidationException_Returns400WithFieldErrors()
    {
        var (status, body) = await RunAsync(new BusinessValidationException("HireDate", "Hire date cannot be in the future."));

        Assert.Equal(StatusCodes.Status400BadRequest, status);
        Assert.Equal(
            "Hire date cannot be in the future.",
            body.GetProperty("errors").GetProperty("HireDate")[0].GetString());
    }

    [Fact]
    public async Task InvokeAsync_UnexpectedException_Returns500WithoutLeakingDetails()
    {
        var (status, body) = await RunAsync(new InvalidOperationException("connection string is Server=secret"));

        Assert.Equal(StatusCodes.Status500InternalServerError, status);
        Assert.DoesNotContain("secret", body.GetProperty("detail").GetString());
    }

    [Fact]
    public async Task InvokeAsync_NoException_PassesThroughUntouched()
    {
        var context = new DefaultHttpContext();
        var sut = new ExceptionHandlingMiddleware(
            ctx =>
            {
                ctx.Response.StatusCode = StatusCodes.Status204NoContent;
                return Task.CompletedTask;
            },
            NullLogger<ExceptionHandlingMiddleware>.Instance);

        await sut.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status204NoContent, context.Response.StatusCode);
    }
}
