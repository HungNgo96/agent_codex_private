namespace AgentTeams.SampleApi.Infrastructure.Employees;

public sealed record EmployeeDatabaseSettings(string Provider, string ConnectionStringName)
{
    public bool PostgreSqlSupported => true;

    public bool AutoMigrationsEnabled => Provider == "sqlite";
}
