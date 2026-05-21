using AgentTeams.SampleApi.Application.Employees.Ports;

namespace AgentTeams.SampleApi.Application.Employees.UseCases;

public sealed class ListEmployeesUseCase(IEmployeeRepository repository)
{
    public async Task<EmployeePageResponse> ExecuteAsync(
        int page = 1,
        int pageSize = 20,
        string? q = null,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var search = string.IsNullOrWhiteSpace(q) ? null : q.Trim();

        var result = await repository.ListAsync(
            new EmployeeListQuery(page, pageSize, search),
            cancellationToken);

        return new EmployeePageResponse(
            result.Items.Select(employee => employee.ToDto()).ToList(),
            result.Total,
            page,
            pageSize);
    }
}
