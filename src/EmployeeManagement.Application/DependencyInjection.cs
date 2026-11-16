using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Scoped: services depend on scoped repositories, so they must also be scoped
        // (a singleton here would be a captive dependency).
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
