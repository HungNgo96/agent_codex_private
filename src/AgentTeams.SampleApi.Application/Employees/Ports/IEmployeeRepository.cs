using AgentTeams.SampleApi.Domain.Employees;

namespace AgentTeams.SampleApi.Application.Employees.Ports;

public interface IEmployeeRepository
{
    Task<bool> EmployeeCodeExistsAsync(string employeeCode, CancellationToken cancellationToken);

    Task<Employee?> FindByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<EmployeePage> ListAsync(EmployeeListQuery query, CancellationToken cancellationToken);

    Task AddAsync(Employee employee, CancellationToken cancellationToken);

    void Remove(Employee employee);
}

public sealed record EmployeeListQuery(int Page, int PageSize, string? Search);

public sealed record EmployeePage(IReadOnlyList<Employee> Items, int Total);
