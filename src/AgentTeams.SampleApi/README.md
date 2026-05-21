# AgentTeams.SampleApi

Minimal .NET 10 REST API example for the Agent Teams template.

The employee feature uses Clean Architecture:

- `AgentTeams.SampleApi`: API host and Minimal API inbound adapters.
- `AgentTeams.SampleApi.Domain`: employee domain model and rules.
- `AgentTeams.SampleApi.Application`: use cases, DTOs, result types, and ports.
- `AgentTeams.SampleApi.Infrastructure`: EF Core repositories, SQLite/PostgreSQL provider wiring, and migrations.

## Run

```powershell
dotnet run --project src\AgentTeams.SampleApi\AgentTeams.SampleApi.csproj
```

## Try

```powershell
curl http://localhost:5062/api/v1/products
curl http://localhost:5062/api/v1/products/1
curl http://localhost:5062/api/v1/agent-workflows
curl http://localhost:5062/api/v1/agent-workflows/1
curl http://localhost:5062/api/v1/harness-runs
curl http://localhost:5062/api/v1/harness-runs/20260521-100730
curl http://localhost:5062/api/v1/employees
curl http://localhost:5062/api/v1/employees/storage
curl http://localhost:5062/openapi/v1.json
```

Create an employee:

```powershell
curl -Method POST http://localhost:5062/api/v1/employees `
  -ContentType "application/json" `
  -Body '{"employeeCode":"EMP-001","fullName":"Nguyen Van A","email":"nguyen.van.a@example.com","department":"Engineering","jobTitle":"Backend Engineer"}'
```

## Example APIs

- `GET /api/v1/products`: basic sample resource.
- `GET /api/v1/agent-workflows`: example lead/worker/handoff/verification flow.
- `GET /api/v1/harness-runs`: example harness results that match this repository's local QA flow.
- `GET /api/v1/employees`: employee management API backed by SQLite by default.

## Employee Database

The employee API uses EF Core migrations with SQLite by default:

```json
{
  "Database": {
    "Provider": "Sqlite"
  },
  "ConnectionStrings": {
    "EmployeeDatabase": "Data Source=employees.db"
  }
}
```

To use PostgreSQL later, set:

```json
{
  "Database": {
    "Provider": "PostgreSql"
  },
  "ConnectionStrings": {
    "EmployeeDatabase": "Host=localhost;Database=agent_teams;Username=postgres;Password=postgres"
  }
}
```

The API host runs pending employee migrations on startup for SQLite. PostgreSQL provider wiring is available through configuration, but PostgreSQL migrations should be generated and reviewed separately before applying to a PostgreSQL database.

## Agent-Team Scope

- Lead owns project setup, integration, and final verification.
- API worker scope is `src/AgentTeams.SampleApi`.
- Review worker scope is build and endpoint verification.

Completion requires a successful build and a local endpoint check through the harness.
