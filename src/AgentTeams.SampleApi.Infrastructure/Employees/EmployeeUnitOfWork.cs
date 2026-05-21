using AgentTeams.SampleApi.Application.Employees.Ports;

namespace AgentTeams.SampleApi.Infrastructure.Employees;

public sealed class EmployeeUnitOfWork(EmployeeDbContext dbContext) : IEmployeeUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
