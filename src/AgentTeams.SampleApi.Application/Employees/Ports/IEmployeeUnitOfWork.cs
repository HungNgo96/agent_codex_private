namespace AgentTeams.SampleApi.Application.Employees.Ports;

public interface IEmployeeUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
