using AgentTeams.SampleApi.Application.Employees.Ports;

namespace AgentTeams.SampleApi.Application.Employees.UseCases;

public sealed class UpdateEmployeeUseCase(
    IEmployeeRepository repository,
    IEmployeeUnitOfWork unitOfWork,
    IEmployeeClock clock)
{
    public async Task<EmployeeResult<EmployeeDto>> ExecuteAsync(
        Guid id,
        UpdateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var employee = await repository.FindByIdAsync(id, cancellationToken);
        if (employee is null)
        {
            return EmployeeResult<EmployeeDto>.NotFound();
        }

        var updateResult = employee.Update(
            request.FullName,
            request.Email,
            request.Department,
            request.JobTitle,
            request.IsActive,
            clock.UtcNow);
        if (!updateResult.IsValid)
        {
            return EmployeeResult<EmployeeDto>.ValidationFailed(updateResult.ValidationErrors);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return EmployeeResult<EmployeeDto>.Success(employee.ToDto());
    }
}
