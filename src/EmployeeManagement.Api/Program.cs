using System.Text;
using EmployeeManagement.Api.Authorization;
using EmployeeManagement.Api.Middleware;
using EmployeeManagement.Api.OpenApi;
using EmployeeManagement.Application;
using EmployeeManagement.Infrastructure;
using EmployeeManagement.Infrastructure.Authentication;
using EmployeeManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ---- Services (dependency injection container) ----
builder.Services.AddControllers();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// OpenAPI document generation (served at /openapi/v1.json, browsed through Swagger UI).
builder.Services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());

// Authentication (who are you?) using JWT bearer tokens.
var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? new JwtSettings();

if (string.IsNullOrWhiteSpace(jwtSettings.Key) || jwtSettings.Key.Length < 32)
{
    throw new InvalidOperationException(
        "Configuration value 'Jwt:Key' is missing or shorter than 32 characters. " +
        "Set it with user-secrets or an environment variable; see README.md.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            NameClaimType = ClaimNames.Name,
            RoleClaimType = ClaimNames.Role
        };
    });

// Authorization (what may you do?) using a role-based policy.
builder.Services.AddAuthorizationBuilder()
    .AddPolicy(AuthorizationPolicies.AdminOnly, policy => policy.RequireRole(Roles.Admin));

var app = builder.Build();

// ---- HTTP request pipeline (order matters) ----
// 1. Exception handling first, so it wraps everything below it.
app.UseMiddleware<ExceptionHandlingMiddleware>();

// 2. Request logging.
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Employee Management API v1"));

    // Create/upgrade the database from migrations when developing locally.
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
}

// 3. HTTPS redirection.
app.UseHttpsRedirection();

// 4. Authentication must run before authorization.
app.UseAuthentication();
app.UseAuthorization();

// 5. Endpoints.
app.MapControllers();

app.Run();
