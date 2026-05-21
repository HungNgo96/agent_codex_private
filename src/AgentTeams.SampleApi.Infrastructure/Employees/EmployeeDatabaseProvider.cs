using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AgentTeams.SampleApi.Infrastructure.Employees;

internal static class EmployeeDatabaseProvider
{
    private const string ConnectionStringName = "EmployeeDatabase";
    private const string DefaultProvider = "sqlite";
    private const string DefaultConnectionString = "Data Source=employees.db";

    public static EmployeeDatabaseSettings Resolve(IConfiguration configuration)
    {
        var provider = NormalizeProvider(configuration["Database:Provider"]);
        return new EmployeeDatabaseSettings(provider, ConnectionStringName);
    }

    public static void Configure(DbContextOptionsBuilder options, IConfiguration configuration)
    {
        var provider = NormalizeProvider(configuration["Database:Provider"]);
        var connectionString = configuration.GetConnectionString(ConnectionStringName);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = DefaultConnectionString;
        }

        if (provider == "postgresql")
        {
            options.UseNpgsql(connectionString);
            return;
        }

        if (provider == "sqlite")
        {
            options.UseSqlite(connectionString);
            return;
        }

        throw new InvalidOperationException(
            $"Unsupported employee database provider '{provider}'. Use 'sqlite' or 'postgresql'.");
    }

    private static string NormalizeProvider(string? provider)
    {
        var normalized = provider?.Trim().ToLowerInvariant();
        return normalized switch
        {
            null or "" => DefaultProvider,
            "postgres" or "npgsql" => "postgresql",
            _ => normalized
        };
    }
}
