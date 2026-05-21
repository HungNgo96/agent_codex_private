using AgentTeams.SampleApi.Application.Employees.Ports;

namespace AgentTeams.SampleApi.Application.Employees.UseCases;

public sealed class DeleteEmployeeUseCase(
    IEmployeeRepository repository,
    IEmployeeUnitOfWork unitOfWork)
{
    public async Task<EmployeeResult> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var employee = await repository.FindByIdAsync(id, cancellationToken);
        if (employee is null)
        {
            return EmployeeResult.NotFound();
        }

        repository.Remove(employee);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return EmployeeResult.Success();
    }
}
