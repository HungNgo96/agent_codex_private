using AgentTeams.SampleApi.Application.Employees;
using AgentTeams.SampleApi.Application.Employees.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AgentTeams.SampleApi.Employees;

public static class EmployeeEndpoints
{
    public static RouteGroupBuilder MapEmployeeEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/employees")
            .WithTags("Employees");

        group.MapGet("/", async Task<Ok<EmployeePageResponse>> (
            ListEmployeesUseCase useCase,
            int page = 1,
            int pageSize = 20,
            string? q = null,
            CancellationToken cancellationToken = default) =>
        {
            var response = await useCase.ExecuteAsync(page, pageSize, q, cancellationToken);
            return TypedResults.Ok(response);
        })
            .WithName("GetEmployees")
            .WithSummary("Get employees with simple paging and search");

        group.MapGet("/{id:guid}", async Task<Results<Ok<EmployeeDto>, NotFound>> (
            Guid id,
            GetEmployeeByIdUseCase useCase,
            CancellationToken cancellationToken) =>
        {
            var result = await useCase.ExecuteAsync(id, cancellationToken);
            return result.Status == EmployeeResultStatus.Success
                ? TypedResults.Ok(result.Value!)
                : TypedResults.NotFound();
        })
            .WithName("GetEmployeeById")
            .WithSummary("Get one employee by id");

        group.MapPost("/", async Task<IResult> (
            CreateEmployeeRequest request,
            CreateEmployeeUseCase useCase,
            CancellationToken cancellationToken) =>
        {
            var result = await useCase.ExecuteAsync(request, cancellationToken);
            return ToHttpResult(result, employee =>
                Results.Created($"/api/v1/employees/{employee.Id}", employee));
        })
            .WithName("CreateEmployee")
            .WithSummary("Create an employee");

        group.MapPost("/basic-info", async Task<IResult> (
            CreateEmployeeBasicInfoRequest request,
            CreateEmployeeBasicInfoUseCase useCase,
            CancellationToken cancellationToken) =>
        {
            var result = await useCase.ExecuteAsync(request, cancellationToken);
            return ToHttpResult(result, employee =>
                Results.Created($"/api/v1/employees/{employee.Id}", employee));
        })
            .WithName("CreateEmployeeBasicInfo")
            .WithSummary("Create an employee from basic information");

        group.MapPut("/{id:guid}", async Task<IResult> (
            Guid id,
            UpdateEmployeeRequest request,
            UpdateEmployeeUseCase useCase,
            CancellationToken cancellationToken) =>
        {
            var result = await useCase.ExecuteAsync(id, request, cancellationToken);
            return ToHttpResult(result, Results.Ok);
        })
            .WithName("UpdateEmployee")
            .WithSummary("Update an employee");

        group.MapDelete("/{id:guid}", async Task<IResult> (
            Guid id,
            DeleteEmployeeUseCase useCase,
            CancellationToken cancellationToken) =>
        {
            var result = await useCase.ExecuteAsync(id, cancellationToken);
            return result.Status == EmployeeResultStatus.Success
                ? Results.NoContent()
                : Results.NotFound();
        })
            .WithName("DeleteEmployee")
            .WithSummary("Delete an employee");

        group.MapGet("/storage", (GetEmployeeStorageMetadataUseCase useCase) =>
                TypedResults.Ok(useCase.Execute()))
            .WithName("GetEmployeeStorage")
            .WithSummary("Get employee database provider metadata");

        return group;
    }

    private static IResult ToHttpResult<T>(
        EmployeeResult<T> result,
        Func<T, IResult> success)
    {
        return result.Status switch
        {
            EmployeeResultStatus.Success => success(result.Value!),
            EmployeeResultStatus.ValidationFailed => Results.ValidationProblem(
                result.ValidationErrors.ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value)),
            EmployeeResultStatus.NotFound => Results.NotFound(),
            EmployeeResultStatus.Conflict => Results.Conflict(new
            {
                code = result.Code,
                message = result.Message
            }),
            _ => Results.Problem("Unexpected employee result status.")
        };
    }
}
