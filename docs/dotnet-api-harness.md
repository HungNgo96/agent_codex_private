# .NET API Harness

This harness gives agents a runnable verification loop for the sample .NET API under `src`.

## Command

Use the unified agent harness runner for normal agent work:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/Invoke-AgentHarness.ps1 -Mode Quick
powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/Invoke-AgentHarness.ps1 -Mode Full
powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/Invoke-AgentHarness.ps1 -Mode Status
```

Use `Quick` during narrow implementation loops. Use `Full` before final reporting when API behavior changed. Use `Status` when resuming or reviewing the latest harness evidence.

The API-specific full harness remains available directly:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\run-dotnet-api-harness.ps1
```

Use `-Port` when `5062` is already in use:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\run-dotnet-api-harness.ps1 -Port 5070
```

## What It Verifies

`Quick` verifies:

- Builds `src\AgentTeams.SampleApi\AgentTeams.SampleApi.csproj`.
- Tests `src\AgentTeams.SampleApi.Tests\AgentTeams.SampleApi.Tests.csproj`.
- Runs `git diff --check`.

`Full` delegates to the API harness, which:

- Builds `src\AgentTeams.SampleApi\AgentTeams.SampleApi.csproj`.
- Tests `src\AgentTeams.SampleApi.Tests\AgentTeams.SampleApi.Tests.csproj`.
- Starts the API with isolated JWT auth settings and a SQLite database in `.harness-runs\<run-id>\employees.db`.
- Waits for `/openapi/v1.json`.
- Gets a development bearer token from `/api/v1/auth/dev-token`.
- Probes product, agent workflow, sample harness, employee list, employee storage, authenticated basic-info creation, and employee read-back endpoints.
- Stops the API process after the run.

## Evidence

Each `Quick` or `Full` run writes evidence to `.harness-runs\<run-id>\`:

- `result.json`: run status, command outcomes, endpoint outcomes, base URL, and log paths.
- `build.log`: build output.
- `test.log`: test output.
- `diff-check.log`: Quick-mode whitespace and conflict-marker check output.
- `api.stdout.log` and `api.stderr.log`: Full-mode API process logs.

Agents must report the `result.json` path and the command outcome before claiming the harness passed.

## Agent Flow

- Coder: run `Invoke-AgentHarness.ps1 -Mode Quick` or focused tests while changing API behavior.
- Lead: run `Invoke-AgentHarness.ps1 -Mode Full` before final reporting when API behavior changed.
- Reviewer or evaluator: inspect the final diff and the latest harness evidence.

If the harness fails, preserve `.harness-runs\<run-id>\`, report the failing command or endpoint, and do not claim completion.
