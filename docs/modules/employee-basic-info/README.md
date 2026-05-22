# Employee Basic Info Module

This module owns the example flow for saving basic employee information in the sample API.

## Owned Areas

- `src/AgentTeams.SampleApi.Application/Employees/EmployeeContracts.cs`
- `src/AgentTeams.SampleApi.Application/Employees/UseCases/CreateEmployeeBasicInfoUseCase.cs`
- `src/AgentTeams.SampleApi/Employees/EmployeeEndpoints.cs`
- `src/AgentTeams.SampleApi/Program.cs`
- `src/AgentTeams.SampleApi.Tests/EmployeeUseCaseTests.cs`
- `src/AgentTeams.SampleApi.Tests/EmployeeApiTests.cs`

## Behavior

`POST /api/v1/employees/basic-info` accepts `EmployeeCode`, `FullName`, and `Email`, then stores an employee using the existing employee domain model.

The endpoint defaults fields that are required by the existing employee aggregate but are not part of the basic-info request:

- `Department`: `General`
- `JobTitle`: `Employee`
