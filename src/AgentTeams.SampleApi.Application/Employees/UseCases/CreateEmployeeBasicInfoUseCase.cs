namespace AgentTeams.SampleApi.Application.Employees.UseCases;

public sealed class CreateEmployeeBasicInfoUseCase(CreateEmployeeUseCase createEmployeeUseCase)
{
    private const string DefaultDepartment = "General";
    private const string DefaultJobTitle = "Employee";

    public Task<EmployeeResult<EmployeeDto>> ExecuteAsync(
        CreateEmployeeBasicInfoRequest request,
        CancellationToken cancellationToken)
    {
        var createRequest = new CreateEmployeeRequest(
            request.EmployeeCode,
            request.FullName,
            request.Email,
            DefaultDepartment,
            DefaultJobTitle);

        return createEmployeeUseCase.ExecuteAsync(createRequest, cancellationToken);
    }
}
