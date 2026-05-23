# Agent Workflow History

## 2026-05-23 - Add Runnable .NET API Harness

- Scope: Add a PowerShell harness that builds, tests, starts, probes, and records evidence for the sample .NET API under `src`.
- Files changed: `scripts/run-dotnet-api-harness.ps1`, `docs/dotnet-api-harness.md`, `src/AgentTeams.SampleApi/README.md`, `docs/harness-coding-agent-flow.md`, and this module history.
- Verification: `dotnet test src\AgentTeams.SampleApi.Tests\AgentTeams.SampleApi.Tests.csproj`, `powershell -ExecutionPolicy Bypass -File .\scripts\run-dotnet-api-harness.ps1`, Markdown reference checks, and `git diff --check`.
- Follow-up: Keep future API features covered by either focused tests or a harness endpoint probe before final reporting.

## 2026-05-23 - Add Harness Coding Agent Flow

- Scope: Add a harness-engineering guide that maps external Codex and Claude harness practices into the AgentTeams workflow.
- Files changed: `docs/harness-coding-agent-flow.md`, `docs/agent-ai-task-flow.md`, `README.md`, `ARCHITECTURE.md`, and this module history.
- Verification: Markdown reference checks and `git diff --check`.
- Follow-up: Consider adding a task-specific feature ledger template if repeated long-running work needs pass/fail acceptance tracking.

## 2026-05-23 - Optimize Cross-Platform Coding Plan

- Scope: Make the optimized Agent AI coding lifecycle explicit across shared docs, prompts, workflows, templates, and platform adapters.
- Files changed: `AGENTS.md`, `README.md`, `docs/agent-ai-task-flow.md`, platform role adapters, prompts, workflows, templates, and this module history.
- Verification: Static artifact inventory, role metadata checks, stale-reference searches, Markdown local-link checks, and `git diff --check`.
- Follow-up: Keep future role changes synchronized across Codex, Cursor, and Claude Code adapters.

## 2026-05-22 - Simplify AgentTeams Structure

- Scope: Reduce conceptual overhead while preserving Codex, Cursor, and Claude compatibility.
- Files changed: root docs, Codex/Cursor/Claude workflow docs, module-history docs, and repository cleanup.
- Verification: Static file inventory, reference search, and metadata checks.
- Follow-up: Keep future module task logs under `docs/modules/<module>/history.md`.

## 2026-05-22 - Align Subagent Roles With Teammate Standard

- Scope: Tighten Codex role instructions after reviewing OpenAI subagent guidance.
- Files changed: `.codex/agents/leader.toml`, `.codex/agents/coder.toml`, `.codex/agents/reviewer.toml`, `docs/agent-ai-task-flow.md`, and this module history.
- Verification: Codex agent metadata check, Markdown link check, and `git diff --check`.
- Follow-up: Keep role instructions narrow and update this module when workflow expectations change.
