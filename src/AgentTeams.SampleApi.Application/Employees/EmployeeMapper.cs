using AgentTeams.SampleApi.Domain.Employees;

namespace AgentTeams.SampleApi.Application.Employees;

internal static class EmployeeMapper
{
    public static EmployeeDto ToDto(this Employee employee)
    {
        return new EmployeeDto(
            employee.Id,
            employee.EmployeeCode,
            employee.FullName,
            employee.Email,
            employee.Department,
            employee.JobTitle,
            employee.IsActive,
            employee.CreatedAt,
            employee.UpdatedAt);
    }
}
