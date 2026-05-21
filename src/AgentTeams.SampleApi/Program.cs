using Microsoft.AspNetCore.Http.HttpResults;
using AgentTeams.SampleApi.Employees;
using AgentTeams.SampleApi.Application.Employees.UseCases;
using AgentTeams.SampleApi.Infrastructure.Employees;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEmployeeInfrastructure(builder.Configuration);
builder.Services.AddScoped<CreateEmployeeUseCase>();
builder.Services.AddScoped<DeleteEmployeeUseCase>();
builder.Services.AddScoped<GetEmployeeByIdUseCase>();
builder.Services.AddScoped<GetEmployeeStorageMetadataUseCase>();
builder.Services.AddScoped<ListEmployeesUseCase>();
builder.Services.AddScoped<UpdateEmployeeUseCase>();

var app = builder.Build();
await app.Services.MigrateEmployeeDatabaseAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

ProductDto[] products =
[
    new(1, "Mechanical Keyboard", 89.99m),
    new(2, "Wireless Mouse", 34.50m),
    new(3, "USB-C Dock", 129.00m)
];

AgentWorkflowDto[] workflows =
[
    new(1, "intake", "Read AGENTS.md, architecture docs, and task-specific workflow guidance.", "Lead"),
    new(2, "dispatch", "Assign bounded worker scopes only when parallel work is useful.", "Lead"),
    new(3, "handoff", "Return changed files, evidence, verification, risks, and next step.", "Worker"),
    new(4, "verify", "Run the harness and report the generated evidence before claiming completion.", "Lead")
];

HarnessRunDto[] harnessRuns =
[
    new("20260521-100730", "Full", "Passed", 8, 0, 0, "Full harness with sample API endpoint probes."),
    new("20260521-100719", "Quick", "Passed", 7, 0, 0, "Static docs, knowledge-store, plan, and build checks.")
];

var api = app.MapGroup("/api/v1/products")
    .WithTags("Products");

api.MapGet("/", () => TypedResults.Ok(products))
    .WithName("GetProducts")
    .WithSummary("Get all products");

api.MapGet("/{id:int}", Results<Ok<ProductDto>, NotFound> (int id) =>
{
    var product = products.FirstOrDefault(product => product.Id == id);

    return product is null
        ? TypedResults.NotFound()
        : TypedResults.Ok(product);
})
    .WithName("GetProductById")
    .WithSummary("Get one product by id");

var workflowApi = app.MapGroup("/api/v1/agent-workflows")
    .WithTags("Agent Workflows");

workflowApi.MapGet("/", () => TypedResults.Ok(workflows))
    .WithName("GetAgentWorkflows")
    .WithSummary("Get example agent-team workflow steps");

workflowApi.MapGet("/{id:int}", Results<Ok<AgentWorkflowDto>, NotFound> (int id) =>
{
    var workflow = workflows.FirstOrDefault(workflow => workflow.Id == id);

    return workflow is null
        ? TypedResults.NotFound()
        : TypedResults.Ok(workflow);
})
    .WithName("GetAgentWorkflowById")
    .WithSummary("Get one agent-team workflow step by id");

var harnessApi = app.MapGroup("/api/v1/harness-runs")
    .WithTags("Harness Runs");

harnessApi.MapGet("/", () => TypedResults.Ok(harnessRuns))
    .WithName("GetHarnessRuns")
    .WithSummary("Get example harness run results");

harnessApi.MapGet("/{id}", Results<Ok<HarnessRunDto>, NotFound> (string id) =>
{
    var run = harnessRuns.FirstOrDefault(run => string.Equals(run.Id, id, StringComparison.OrdinalIgnoreCase));

    return run is null
        ? TypedResults.NotFound()
        : TypedResults.Ok(run);
})
    .WithName("GetHarnessRunById")
    .WithSummary("Get one example harness run result by id");

app.MapEmployeeEndpoints();

app.Run();

public partial class Program;

public sealed record ProductDto(int Id, string Name, decimal Price);

public sealed record AgentWorkflowDto(int Id, string Stage, string Description, string Owner);

public sealed record HarnessRunDto(
    string Id,
    string Mode,
    string Status,
    int Passed,
    int Failed,
    int Blocked,
    string Summary);
