# Codex Configuration History

## 2026-05-23 - Add Harness Execution-Surface Policy

- Scope: Add MCP, permission-profile, Windows Codex CLI, and task-level tool assumption guidance for the Codex harness.
- Files changed: `docs/modules/codex-configuration/README.md`, `docs/modules/codex-configuration/rules.md`, `templates/task-brief.md`, and this module history.
- Verification: Inspect changed Markdown, run stale-reference searches, verify Codex command-rule checks through `codex.cmd`, and inspect git diff.
- Follow-up: When a real MCP server is added, document its owner, tool allow-list, approval policy, environment variables, and side effects before enabling it.

## 2026-05-23 - Align Codex Agent Roles With Optimized Coding Plan

- Scope: Align Codex custom agents and configuration guidance with the cross-platform optimized coding lifecycle.
- Files changed: `.codex/agents/*`, `docs/codex-flow-audit.md`, `docs/modules/codex-configuration/*`, and platform references that depend on Codex role behavior.
- Verification: Codex agent metadata checks, stale-reference searches, Markdown local-link checks, and `git diff --check`.
- Follow-up: Keep `.codex/config.toml` conservative and update config only when the Codex docs or runtime behavior require it.

## 2026-05-22 - Clarify User-Level Codex Profiles

- Scope: Clarify that `plan`, `code`, and `review` profiles are user-level shortcuts in the current setup, while project config keeps shared defaults, agents, rules, and fan-out limits.
- Files changed: `docs/modules/codex-configuration/*`, `docs/codex-flow-audit.md`, and `docs/agent-ai-task-flow.md`.
- Verification: Search profile references and inspect git diff.
- Follow-up: Keep `.codex/config.toml` profile blocks out of the project template unless Codex supports project-level profile definitions for this use case.

## 2026-05-22 - Optimize Codex Coding Configuration

- Scope: Add Codex project defaults and document configuration choices for Agent AI coding flow.
- Files changed: `.codex/config.toml`, `docs/modules/codex-configuration/*`, `AGENTS.md`, `docs/codex-flow-audit.md`, and `docs/agent-ai-task-flow.md`.
- Verification: Parse `.codex/config.toml`, verify required Codex agents, check Markdown links, and inspect git diff.
- Follow-up: Add hooks or rules only if repeated manual checks become stable enough to automate.

## 2026-05-22 - Add Codex Command Rules

- Scope: Add project-local Codex command rules for routine verification and destructive-command protection.
- Files changed: `.codex/rules/default.rules`, `docs/modules/codex-configuration/*`.
- Verification: Check rule docs against OpenAI Codex Rules behavior, validate local Markdown links, and inspect git diff.
- Follow-up: Test individual policies with `codex execpolicy check` whenever a rule changes.

## 2026-05-22 - Add Codex Coding Skill

- Scope: Add a repo-local Codex skill for the repeatable AgentTeams feature-development workflow.
- Files changed: `.agents/skills/agent-ai-feature-development/SKILL.md`, `README.md`, `docs/codex-flow-audit.md`, and `docs/modules/codex-configuration/*`.
- Verification: Check skill metadata, validate local Markdown links, and inspect git diff.
- Follow-up: Add more skills only when a repeated workflow has clear trigger wording and cannot stay simpler as docs.
