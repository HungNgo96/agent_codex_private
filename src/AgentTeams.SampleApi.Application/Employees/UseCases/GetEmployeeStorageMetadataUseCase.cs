using AgentTeams.SampleApi.Application.Employees.Ports;

namespace AgentTeams.SampleApi.Application.Employees.UseCases;

public sealed class GetEmployeeStorageMetadataUseCase(IEmployeeStorageMetadataProvider metadataProvider)
{
    public EmployeeStorageResponse Execute()
    {
        return metadataProvider.GetMetadata();
    }
}
