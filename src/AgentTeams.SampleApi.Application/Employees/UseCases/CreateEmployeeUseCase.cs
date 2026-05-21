using AgentTeams.SampleApi.Application.Employees.Ports;
using AgentTeams.SampleApi.Domain.Employees;

namespace AgentTeams.SampleApi.Application.Employees.UseCases;

public sealed class CreateEmployeeUseCase(
    IEmployeeRepository repository,
    IEmployeeUnitOfWork unitOfWork,
    IEmployeeClock clock)
{
    public async Task<EmployeeResult<EmployeeDto>> ExecuteAsync(
        CreateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var createResult = Employee.Create(
            request.EmployeeCode,
            request.FullName,
            request.Email,
            request.Department,
            request.JobTitle,
            clock.UtcNow);
        if (!createResult.IsValid)
        {
            return EmployeeResult<EmployeeDto>.ValidationFailed(createResult.ValidationErrors);
        }

        var employee = createResult.Value!;
        if (await repository.EmployeeCodeExistsAsync(employee.EmployeeCode, cancellationToken))
        {
            return EmployeeResult<EmployeeDto>.Conflict(
                "employee_code_exists",
                $"Employee code '{employee.EmployeeCode}' already exists.");
        }

        await repository.AddAsync(employee, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return EmployeeResult<EmployeeDto>.Success(employee.ToDto());
    }
}
