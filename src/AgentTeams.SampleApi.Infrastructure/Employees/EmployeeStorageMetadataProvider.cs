using AgentTeams.SampleApi.Application.Employees;
using AgentTeams.SampleApi.Application.Employees.Ports;

namespace AgentTeams.SampleApi.Infrastructure.Employees;

public sealed class EmployeeStorageMetadataProvider(EmployeeDatabaseSettings settings) : IEmployeeStorageMetadataProvider
{
    public EmployeeStorageResponse GetMetadata()
    {
        return new EmployeeStorageResponse(
            settings.Provider,
            settings.PostgreSqlSupported,
            settings.ConnectionStringName,
            settings.AutoMigrationsEnabled);
    }
}
