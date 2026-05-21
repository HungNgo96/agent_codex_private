namespace AgentTeams.SampleApi.Application.Employees.Ports;

public interface IEmployeeStorageMetadataProvider
{
    EmployeeStorageResponse GetMetadata();
}
