using AgentTeams.SampleApi.Application.Employees.Ports;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AgentTeams.SampleApi.Infrastructure.Employees;

public static class EmployeeDatabaseRegistration
{
    public static IServiceCollection AddEmployeeInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var settings = EmployeeDatabaseProvider.Resolve(configuration);

        services.AddSingleton(settings);
        services.AddSingleton<IEmployeeClock, SystemEmployeeClock>();
        services.AddDbContext<EmployeeDbContext>(options => EmployeeDatabaseProvider.Configure(options, configuration));
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IEmployeeUnitOfWork, EmployeeUnitOfWork>();
        services.AddScoped<IEmployeeStorageMetadataProvider, EmployeeStorageMetadataProvider>();

        return services;
    }

    public static async Task MigrateEmployeeDatabaseAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var settings = scope.ServiceProvider.GetRequiredService<EmployeeDatabaseSettings>();
        if (!settings.AutoMigrationsEnabled)
        {
            return;
        }

        var dbContext = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}

internal sealed class SystemEmployeeClock : IEmployeeClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
