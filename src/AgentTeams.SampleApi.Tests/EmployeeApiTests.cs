using System.Net;
using System.Net.Http.Headers;
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
        await AuthorizeAsync(client);

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
        await AuthorizeAsync(client);

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
    public async Task CreateEmployeeBasicInfoPersistsDefaultsAndCanBeReadBack()
    {
        await using var factory = new EmployeeApiFactory();
        using var client = factory.CreateClient();
        await AuthorizeAsync(client);

        var request = new CreateEmployeeBasicInfoRequest(
            EmployeeCode: "EMP-003",
            FullName: "Le Van C",
            Email: "le.van.c@example.com");

        var createResponse = await client.PostAsJsonAsync("/api/v1/employees/basic-info", request);
        var created = await createResponse.Content.ReadFromJsonAsync<EmployeeResponse>();

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(created);
        Assert.Equal("EMP-003", created.EmployeeCode);
        Assert.Equal("General", created.Department);
        Assert.Equal("Employee", created.JobTitle);

        var getResponse = await client.GetAsync($"/api/v1/employees/{created.Id}");
        var fetched = await getResponse.Content.ReadFromJsonAsync<EmployeeResponse>();

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(fetched);
        Assert.Equal(created.Id, fetched.Id);
        Assert.Equal("Le Van C", fetched.FullName);
        Assert.Equal("le.van.c@example.com", fetched.Email);
        Assert.Equal("General", fetched.Department);
        Assert.Equal("Employee", fetched.JobTitle);
    }

    [Fact]
    public async Task CreateEmployeeBasicInfoRequiresAuthentication()
    {
        await using var factory = new EmployeeApiFactory();
        using var client = factory.CreateClient();

        var request = new CreateEmployeeBasicInfoRequest(
            EmployeeCode: "EMP-004",
            FullName: "Pham Thi D",
            Email: "pham.thi.d@example.com");

        var response = await client.PostAsJsonAsync("/api/v1/employees/basic-info", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateEmployeeBasicInfoRequiresEmployeeWriteScope()
    {
        await using var factory = new EmployeeApiFactory();
        using var client = factory.CreateClient();
        await AuthorizeAsync(client, "employees.read");

        var request = new CreateEmployeeBasicInfoRequest(
            EmployeeCode: "EMP-005",
            FullName: "Hoang Van E",
            Email: "hoang.van.e@example.com");

        var response = await client.PostAsJsonAsync("/api/v1/employees/basic-info", request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task EmployeeReadEndpointsRemainAnonymous()
    {
        await using var factory = new EmployeeApiFactory();
        using var client = factory.CreateClient();

        var listResponse = await client.GetAsync("/api/v1/employees");
        var storageResponse = await client.GetAsync("/api/v1/employees/storage");

        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, storageResponse.StatusCode);
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

    private static async Task AuthorizeAsync(
        HttpClient client,
        params string[] scopes)
    {
        var request = new DevTokenRequest(
            Subject: "api-test",
            Name: "API Test",
            Scopes: scopes.Length == 0 ? ["employees.write"] : scopes);

        var response = await client.PostAsJsonAsync("/api/v1/auth/dev-token", request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var token = await response.Content.ReadFromJsonAsync<DevTokenResponse>();
        Assert.NotNull(token);
        Assert.False(string.IsNullOrWhiteSpace(token.AccessToken));

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            token.TokenType,
            token.AccessToken);
    }

    private sealed class EmployeeApiFactory : WebApplicationFactory<Program>
    {
        private readonly string _databasePath = Path.Combine(
            Path.GetTempPath(),
            $"agent-teams-employees-{Guid.NewGuid():N}.db");
        private readonly string? _previousDatabaseProvider;
        private readonly string? _previousEmployeeConnectionString;
        private readonly string? _previousAuthIssuer;
        private readonly string? _previousAuthAudience;
        private readonly string? _previousAuthSigningKey;

        public EmployeeApiFactory()
        {
            _previousDatabaseProvider = Environment.GetEnvironmentVariable("Database__Provider");
            _previousEmployeeConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__EmployeeDatabase");
            _previousAuthIssuer = Environment.GetEnvironmentVariable("Auth__Issuer");
            _previousAuthAudience = Environment.GetEnvironmentVariable("Auth__Audience");
            _previousAuthSigningKey = Environment.GetEnvironmentVariable("Auth__SigningKey");

            Environment.SetEnvironmentVariable("Database__Provider", "Sqlite");
            Environment.SetEnvironmentVariable("ConnectionStrings__EmployeeDatabase", $"Data Source={_databasePath}");
            Environment.SetEnvironmentVariable("Auth__Issuer", "AgentTeams.Tests");
            Environment.SetEnvironmentVariable("Auth__Audience", "AgentTeams.SampleApi.Tests");
            Environment.SetEnvironmentVariable("Auth__SigningKey", "agent-teams-tests-signing-key-must-be-long-enough");
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((_, configuration) =>
            {
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Auth:Issuer"] = "AgentTeams.Tests",
                    ["Auth:Audience"] = "AgentTeams.SampleApi.Tests",
                    ["Auth:SigningKey"] = "agent-teams-tests-signing-key-must-be-long-enough",
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
            Environment.SetEnvironmentVariable("Auth__Issuer", _previousAuthIssuer);
            Environment.SetEnvironmentVariable("Auth__Audience", _previousAuthAudience);
            Environment.SetEnvironmentVariable("Auth__SigningKey", _previousAuthSigningKey);

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

    private sealed record CreateEmployeeBasicInfoRequest(
        string EmployeeCode,
        string FullName,
        string Email);

    private sealed record DevTokenRequest(
        string Subject,
        string Name,
        IReadOnlyList<string> Scopes);

    private sealed record DevTokenResponse(
        string AccessToken,
        string TokenType,
        DateTimeOffset ExpiresAt);

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
