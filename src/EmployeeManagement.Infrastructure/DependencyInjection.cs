using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Infrastructure.Authentication;
using EmployeeManagement.Infrastructure.Persistence;
using EmployeeManagement.Infrastructure.Persistence.Repositories;
using EmployeeManagement.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext is registered as Scoped by default: one instance per HTTP request.
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        // Scoped: repositories and unit of work share the request's DbContext.
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();

        // Singleton: stateless, one instance for the whole application lifetime.
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddSingleton<IUserStore, ConfiguredUserStore>();

        // Transient: a new lightweight instance every time it is requested.
        services.AddTransient<ITokenService, TokenService>();

        // Options pattern: strongly typed settings.
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<DemoUserSettings>(configuration.GetSection(DemoUserSettings.SectionName));

        return services;
    }
}
