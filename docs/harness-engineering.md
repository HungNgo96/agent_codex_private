# Harness Engineering

This repository now includes a local QA harness for agent-team work. The harness follows the approach described in OpenAI's harness engineering article: keep repository knowledge local, make feedback legible to agents, and enforce repeated expectations mechanically instead of relying on memory.

Source: https://openai.com/index/harness-engineering/

## Goals

- Validate the agent-team operating contract in `AGENTS.md`, prompts, workflows, teams, and templates.
- Validate the in-repository knowledge store layout in `ARCHITECTURE.md` and `docs/`.
- Check that task briefs, handoffs, and implementation plans preserve ownership and verification evidence.
- Build the sample API and optionally probe its local endpoints.
- Produce reports that are easy for a lead agent, worker agent, or human reviewer to inspect.

This harness does not spawn agents. It is a local quality gate for the documentation-first template.

## Commands

Run the quick harness:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\Invoke-AgentHarness.ps1 -Mode Quick
```

Run the full harness with sample API endpoint probes:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\Invoke-AgentHarness.ps1 -Mode Full -RunSampleApi
```

Run the self-tests for the harness implementation:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\harness\Invoke-HarnessSelfTests.ps1
```

PowerShell 7 can also run the scripts with `pwsh` when it is installed.

## Modes

- `Quick`: static Markdown checks plus `dotnet build` for `src/AgentTeams.SampleApi`.
- `Full`: quick checks plus optional endpoint checks when `-RunSampleApi` is provided.

Options:

- `-OutputDir`: output directory for generated reports. Default: `.harness-runs`.
- `-FailOnWarnings`: converts warning checks to failures.
- `-RunSampleApi`: starts the sample API on a free local port and probes the product and OpenAPI endpoints.

Exit codes:

- `0`: no failed or blocked checks.
- `1`: one or more checks failed.
- `2`: one or more checks were blocked by environment setup.

## Reports

Each run writes:

- `.harness-runs/<timestamp>/report.md`
- `.harness-runs/<timestamp>/report.json`

The JSON report shape is:

```json
{
  "runId": "20260521-120000",
  "startedAt": "2026-05-21T12:00:00.0000000+07:00",
  "mode": "Quick",
  "status": "Passed",
  "repoRoot": "D:\\HungNT97\\Private\\agent_codex_private",
  "environment": [],
  "checks": []
}
```

Each check includes `id`, `name`, `status`, `severity`, `message`, and `evidence`.

## Current Checks

- `required-files`: verifies the core template files exist.
- `knowledge-store-layout`: verifies the repository knowledge store files and directories exist.
- `markdown-sections`: verifies required sections in operating docs, prompts, and templates.
- `repo-local-links`: verifies Markdown links to repository-local paths resolve.
- `handoff-contract`: verifies the handoff template captures scope, evidence, verification, risks, integration notes, and next step.
- `plan-contract`: verifies checked-in implementation plans include goal, architecture, tech stack, task checkboxes, and verification.
- `sample-api-build`: builds the sample API.
- `sample-api-endpoints`: in full mode with `-RunSampleApi`, probes `/api/v1/products`, `/api/v1/products/1`, and `/openapi/v1.json`.

## Git Safe Directory

If Git reports dubious ownership for this checkout, the harness records it as an environment warning. It does not mutate global Git config. To fix that outside the harness, run:

```powershell
git config --global --add safe.directory D:/HungNT97/Private/agent_codex_private
```

## Adding Checks

Add new checks in `scripts/harness/AgentHarness.Core.ps1` and return objects through `New-AgentHarnessCheck`. A good check has:

- A stable `id`.
- A concrete pass/fail condition.
- Evidence that tells the next agent exactly where to look.
- No repo mutation.

Keep checks small and mechanical. If a rule requires judgment, document it first; only promote it into the harness once the rule is stable.
