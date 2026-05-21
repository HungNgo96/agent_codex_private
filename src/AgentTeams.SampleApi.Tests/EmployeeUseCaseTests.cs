using AgentTeams.SampleApi.Application.Employees;
using AgentTeams.SampleApi.Application.Employees.Ports;
using AgentTeams.SampleApi.Application.Employees.UseCases;
using AgentTeams.SampleApi.Domain.Employees;

namespace AgentTeams.SampleApi.Tests;

public sealed class EmployeeUseCaseTests
{
    [Fact]
    public async Task CreateEmployeeReturnsConflictWhenEmployeeCodeAlreadyExists()
    {
        var repository = new FakeEmployeeRepository(existingCodes: ["EMP-001"]);
        var useCase = new CreateEmployeeUseCase(
            repository,
            new FakeEmployeeUnitOfWork(),
            new FixedEmployeeClock());

        var result = await useCase.ExecuteAsync(
            new CreateEmployeeRequest(
                "EMP-001",
                "Nguyen Van A",
                "nguyen.van.a@example.com",
                "Engineering",
                "Backend Engineer"),
            CancellationToken.None);

        Assert.Equal(EmployeeResultStatus.Conflict, result.Status);
        Assert.Equal("employee_code_exists", result.Code);
    }

    [Fact]
    public async Task CreateEmployeeReturnsValidationErrorsForInvalidInput()
    {
        var useCase = new CreateEmployeeUseCase(
            new FakeEmployeeRepository(),
            new FakeEmployeeUnitOfWork(),
            new FixedEmployeeClock());

        var result = await useCase.ExecuteAsync(
            new CreateEmployeeRequest("", "", "invalid", "", ""),
            CancellationToken.None);

        Assert.Equal(EmployeeResultStatus.ValidationFailed, result.Status);
        Assert.Contains("employeeCode", result.ValidationErrors.Keys);
        Assert.Contains("fullName", result.ValidationErrors.Keys);
        Assert.Contains("email", result.ValidationErrors.Keys);
    }

    [Fact]
    public async Task ListEmployeesReturnsPagedSearchResults()
    {
        var repository = new FakeEmployeeRepository();
        repository.Seed(Employee.Create(
            "EMP-001",
            "Nguyen Van A",
            "nguyen.van.a@example.com",
            "Engineering",
            "Backend Engineer",
            DateTimeOffset.UtcNow).Value!);
        repository.Seed(Employee.Create(
            "EMP-002",
            "Tran Thi B",
            "tran.thi.b@example.com",
            "People",
            "HR Specialist",
            DateTimeOffset.UtcNow).Value!);

        var useCase = new ListEmployeesUseCase(repository);

        var result = await useCase.ExecuteAsync(
            page: 1,
            pageSize: 10,
            q: "Engineering",
            CancellationToken.None);

        Assert.Equal(1, result.Total);
        Assert.Single(result.Items);
        Assert.Equal("EMP-001", result.Items[0].EmployeeCode);
    }

    private sealed class FixedEmployeeClock : IEmployeeClock
    {
        public DateTimeOffset UtcNow { get; } = new(2026, 5, 21, 0, 0, 0, TimeSpan.Zero);
    }

    private sealed class FakeEmployeeUnitOfWork : IEmployeeUnitOfWork
    {
        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class FakeEmployeeRepository : IEmployeeRepository
    {
        private readonly HashSet<string> _existingCodes;
        private readonly List<Employee> _employees = [];

        public FakeEmployeeRepository(IEnumerable<string>? existingCodes = null)
        {
            _existingCodes = new HashSet<string>(
                existingCodes ?? [],
                StringComparer.OrdinalIgnoreCase);
        }

        public void Seed(Employee employee)
        {
            _employees.Add(employee);
            _existingCodes.Add(employee.EmployeeCode);
        }

        public Task<bool> EmployeeCodeExistsAsync(
            string employeeCode,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(_existingCodes.Contains(employeeCode));
        }

        public Task<Employee?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_employees.FirstOrDefault(employee => employee.Id == id));
        }

        public Task<EmployeePage> ListAsync(
            EmployeeListQuery query,
            CancellationToken cancellationToken)
        {
            var filtered = _employees.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                filtered = filtered.Where(employee =>
                    employee.EmployeeCode.Contains(query.Search, StringComparison.OrdinalIgnoreCase) ||
                    employee.FullName.Contains(query.Search, StringComparison.OrdinalIgnoreCase) ||
                    employee.Department.Contains(query.Search, StringComparison.OrdinalIgnoreCase));
            }

            var items = filtered
                .OrderBy(employee => employee.EmployeeCode)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            return Task.FromResult(new EmployeePage(items, filtered.Count()));
        }

        public Task AddAsync(Employee employee, CancellationToken cancellationToken)
        {
            Seed(employee);
            return Task.CompletedTask;
        }

        public void Remove(Employee employee)
        {
            _employees.Remove(employee);
        }
    }
}
