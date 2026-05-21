using AgentTeams.SampleApi.Application.Employees.Ports;

namespace AgentTeams.SampleApi.Application.Employees.UseCases;

public sealed class GetEmployeeByIdUseCase(IEmployeeRepository repository)
{
    public async Task<EmployeeResult<EmployeeDto>> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var employee = await repository.FindByIdAsync(id, cancellationToken);
        return employee is null
            ? EmployeeResult<EmployeeDto>.NotFound()
            : EmployeeResult<EmployeeDto>.Success(employee.ToDto());
    }
}
