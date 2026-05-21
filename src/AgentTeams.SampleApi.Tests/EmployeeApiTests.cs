using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace AgentTeams.SampleApi.Tests;

public sealed class EmployeeApiTests
{
    [Fact]
    public async Task CreateEmployeePersistsAndCanBeReadBack()
    {
        await using var factory = new EmployeeApiFactory();
        using var client = factory.CreateClient();

        var request = new CreateEmployeeRequest(
            EmployeeCode: "EMP-001",
            FullName: "Nguyen Van A",
            Email: "nguyen.van.a@example.com",
            Department: "Engineering",
            JobTitle: "Backend Engineer");

        var createResponse = await client.PostAsJsonAsync("/api/v1/employees", request);
        var created = await createResponse.Content.ReadFromJsonAsync<EmployeeResponse>();

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(created);
        Assert.Equal("EMP-001", created.EmployeeCode);

        var getResponse = await client.GetAsync($"/api/v1/employees/{created.Id}");
        var fetched = await getResponse.Content.ReadFromJsonAsync<EmployeeResponse>();

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(fetched);
        Assert.Equal(created.Id, fetched.Id);
        Assert.Equal("Engineering", fetched.Department);
    }

    [Fact]
    public async Task CreateEmployeeRejectsDuplicateEmployeeCode()
    {
        await using var factory = new EmployeeApiFactory();
        using var client = factory.CreateClient();

        var request = new CreateEmployeeRequest(
            EmployeeCode: "EMP-002",
            FullName: "Tran Thi B",
            Email: "tran.thi.b@example.com",
            Department: "People",
            JobTitle: "HR Specialist");

        var firstResponse = await client.PostAsJsonAsync("/api/v1/employees", request);
        var duplicateResponse = await client.PostAsJsonAsync("/api/v1/employees", request);

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, duplicateResponse.StatusCode);
    }

    [Fact]
    public async Task EmployeeStorageEndpointReportsSqliteProviderAndPostgresSupport()
    {
        await using var factory = new EmployeeApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/employees/storage");
        var storage = await response.Content.ReadFromJsonAsync<EmployeeStorageResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(storage);
        Assert.Equal("sqlite", storage.Provider);
        Assert.True(storage.PostgreSqlSupported);
        Assert.True(storage.AutoMigrationsEnabled);
    }

    private sealed class EmployeeApiFactory : WebApplicationFactory<Program>
    {
        private readonly string _databasePath = Path.Combine(
            Path.GetTempPath(),
            $"agent-teams-employees-{Guid.NewGuid():N}.db");
        private readonly string? _previousDatabaseProvider;
        private readonly string? _previousEmployeeConnectionString;

        public EmployeeApiFactory()
        {
            _previousDatabaseProvider = Environment.GetEnvironmentVariable("Database__Provider");
            _previousEmployeeConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__EmployeeDatabase");

            Environment.SetEnvironmentVariable("Database__Provider", "Sqlite");
            Environment.SetEnvironmentVariable("ConnectionStrings__EmployeeDatabase", $"Data Source={_databasePath}");
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((_, configuration) =>
            {
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Database:Provider"] = "Sqlite",
                    ["ConnectionStrings:EmployeeDatabase"] = $"Data Source={_databasePath}"
                });
            });
        }

        public override async ValueTask DisposeAsync()
        {
            await base.DisposeAsync();

            Environment.SetEnvironmentVariable("Database__Provider", _previousDatabaseProvider);
            Environment.SetEnvironmentVariable("ConnectionStrings__EmployeeDatabase", _previousEmployeeConnectionString);

            if (File.Exists(_databasePath))
            {
                try
                {
                    File.Delete(_databasePath);
                }
                catch (IOException)
                {
                }
            }
        }
    }

    private sealed record CreateEmployeeRequest(
        string EmployeeCode,
        string FullName,
        string Email,
        string Department,
        string JobTitle);

    private sealed record EmployeeResponse(
        Guid Id,
        string EmployeeCode,
        string FullName,
        string Email,
        string Department,
        string JobTitle,
        bool IsActive);

    private sealed record EmployeeStorageResponse(
        string Provider,
        bool PostgreSqlSupported,
        bool AutoMigrationsEnabled);
}
