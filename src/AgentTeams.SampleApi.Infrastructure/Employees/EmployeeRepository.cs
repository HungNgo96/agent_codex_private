using AgentTeams.SampleApi.Application.Employees.Ports;
using AgentTeams.SampleApi.Domain.Employees;
using Microsoft.EntityFrameworkCore;

namespace AgentTeams.SampleApi.Infrastructure.Employees;

public sealed class EmployeeRepository(EmployeeDbContext dbContext) : IEmployeeRepository
{
    public async Task<bool> EmployeeCodeExistsAsync(string employeeCode, CancellationToken cancellationToken)
    {
        return await dbContext.Employees
            .AnyAsync(employee => employee.EmployeeCode == employeeCode, cancellationToken);
    }

    public async Task<Employee?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Employees
            .FirstOrDefaultAsync(employee => employee.Id == id, cancellationToken);
    }

    public async Task<EmployeePage> ListAsync(EmployeeListQuery query, CancellationToken cancellationToken)
    {
        var employeesQuery = ApplySearch(dbContext.Employees.AsNoTracking(), query.Search);
        var total = await employeesQuery.CountAsync(cancellationToken);
        var employees = await employeesQuery
            .OrderBy(employee => employee.EmployeeCode)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return new EmployeePage(employees, total);
    }

    public Task AddAsync(Employee employee, CancellationToken cancellationToken)
    {
        dbContext.Employees.Add(employee);
        return Task.CompletedTask;
    }

    public void Remove(Employee employee)
    {
        dbContext.Employees.Remove(employee);
    }

    private static IQueryable<Employee> ApplySearch(IQueryable<Employee> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        var term = search.Trim();
        return query.Where(employee =>
            employee.EmployeeCode.Contains(term) ||
            employee.FullName.Contains(term) ||
            employee.Department.Contains(term));
    }
}
