namespace AgentTeams.SampleApi.Application.Employees;

public sealed record CreateEmployeeRequest(
    string EmployeeCode,
    string FullName,
    string Email,
    string Department,
    string JobTitle);

public sealed record UpdateEmployeeRequest(
    string FullName,
    string Email,
    string Department,
    string JobTitle,
    bool IsActive);

public sealed record EmployeeDto(
    Guid Id,
    string EmployeeCode,
    string FullName,
    string Email,
    string Department,
    string JobTitle,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record EmployeePageResponse(
    IReadOnlyList<EmployeeDto> Items,
    int Total,
    int Page,
    int PageSize);

public sealed record EmployeeStorageResponse(
    string Provider,
    bool PostgreSqlSupported,
    string ConnectionStringName,
    bool AutoMigrationsEnabled);
