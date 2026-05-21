using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AgentTeams.SampleApi.Infrastructure.Employees;

public sealed class EmployeeDbContextFactory : IDesignTimeDbContextFactory<EmployeeDbContext>
{
    public EmployeeDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<EmployeeDbContext>();
        var provider = ResolveValue(args, "--provider")
            ?? Environment.GetEnvironmentVariable("Database__Provider")
            ?? "sqlite";
        var connectionString = ResolveValue(args, "--connection")
            ?? Environment.GetEnvironmentVariable("ConnectionStrings__EmployeeDatabase")
            ?? "Data Source=employees.db";

        switch (provider.Trim().ToLowerInvariant())
        {
            case "postgresql":
            case "postgres":
            case "npgsql":
                options.UseNpgsql(connectionString);
                break;
            case "sqlite":
                options.UseSqlite(connectionString);
                break;
            default:
                throw new InvalidOperationException(
                    $"Unsupported employee database provider '{provider}'. Use 'sqlite' or 'postgresql'.");
        }

        return new EmployeeDbContext(options.Options);
    }

    private static string? ResolveValue(string[] args, string name)
    {
        for (var index = 0; index < args.Length; index++)
        {
            var arg = args[index];
            if (arg.Equals(name, StringComparison.OrdinalIgnoreCase) && index + 1 < args.Length)
            {
                return args[index + 1];
            }

            var prefix = $"{name}=";
            if (arg.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return arg[prefix.Length..];
            }
        }

        return null;
    }
}
