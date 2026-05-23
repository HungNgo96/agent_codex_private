# .NET API Harness

This harness gives agents a runnable verification loop for the sample .NET API under `src`.

## Command

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\run-dotnet-api-harness.ps1
```

Use `-Port` when `5062` is already in use:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\run-dotnet-api-harness.ps1 -Port 5070
```

## What It Verifies

The harness:

- Builds `src\AgentTeams.SampleApi\AgentTeams.SampleApi.csproj`.
- Tests `src\AgentTeams.SampleApi.Tests\AgentTeams.SampleApi.Tests.csproj`.
- Starts the API with an isolated SQLite database in `.harness-runs\<run-id>\employees.db`.
- Waits for `/openapi/v1.json`.
- Probes product, agent workflow, sample harness, employee list, employee storage, basic-info creation, and employee read-back endpoints.
- Stops the API process after the run.

## Evidence

Each run writes evidence to `.harness-runs\<run-id>\`:

- `result.json`: run status, command outcomes, endpoint outcomes, base URL, and log paths.
- `build.log`: build output.
- `test.log`: test output.
- `api.stdout.log` and `api.stderr.log`: API process logs.

Agents must report the `result.json` path and the command outcome before claiming the API harness passed.

## Agent Flow

- Coder: run focused tests while changing API behavior.
- Lead: run the full harness before final reporting.
- Reviewer or evaluator: inspect the final diff and the latest harness evidence.

If the harness fails, preserve `.harness-runs\<run-id>\`, report the failing command or endpoint, and do not claim completion.
