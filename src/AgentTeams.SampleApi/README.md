# AgentTeams.SampleApi

Minimal .NET 10 REST API example for the Agent Teams template.

## Run

```powershell
dotnet run --project src\AgentTeams.SampleApi\AgentTeams.SampleApi.csproj
```

## Try

```powershell
curl http://localhost:5062/api/v1/products
curl http://localhost:5062/api/v1/products/1
curl http://localhost:5062/openapi/v1.json
```

## Agent-Team Scope

- Lead owns project setup, integration, and final verification.
- API worker scope is `src/AgentTeams.SampleApi`.
- Review worker scope is build and endpoint verification.

Completion requires a successful build and a local endpoint check.
