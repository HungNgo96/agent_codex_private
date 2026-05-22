# Agent AI Task Flow

This document describes the flow for handling a new task from a user. It applies to the lead agent, worker agents, and the platform adapters in this repository.

## Contract Sources

- [`AGENTS.md`](../AGENTS.md): shared operating contract for all agents.
- [`prompts/lead-agent.md`](../prompts/lead-agent.md): lead agent prompt.
- [`prompts/worker-agent.md`](../prompts/worker-agent.md): worker agent prompt.
- [`templates/task-brief.md`](../templates/task-brief.md): task brief and delegation decision format.
- [`templates/handoff-note.md`](../templates/handoff-note.md): worker handoff format.
- [`docs/harness-engineering.md`](harness-engineering.md): local harness for verifying docs, contracts, plans, and the sample API.
- [`docs/codex-flow-audit.md`](codex-flow-audit.md): Codex-specific guidance for context loading, subagents, skills, sandboxing, and docs-only verification.

## End-to-End Flow

```text
User task
  |
  v
Lead receives task
  |
  v
Load repo context and define success criteria
  |
  v
Decide delegation: single agent or subagent workflow
  |
  +--> Single agent path: lead keeps work local
  |
  +--> Subagent path: lead writes task brief, assigns bounded workers
  |
  v
Implementation or research work
  |
  v
Worker handoffs, if workers were used
  |
  v
Lead reviews, integrates, and resolves conflicts
  |
  v
Run verification
  |
  v
Final response with changed files, evidence, risks, and next steps
```

## Phase 1: Receive The Task And Load Context

The lead agent starts by reading the user's request and identifying:

- The target outcome.
- Observable or verifiable success criteria.
- Editable scope and avoid scope.
- Contract files that must be read before deciding how to proceed.

The lead must read `AGENTS.md` and explicitly read or attach the relevant workflow, team, prompt, template, or doc before assigning work. In Codex, do not assume these supporting files are loaded automatically just because they exist in the repository. If the task is small, tightly coupled, or depends on one sequential investigation path, the lead keeps the work local instead of using subagents.

## Phase 2: Delegation Decision

The lead must record the delegation decision in the task brief:

- `Use subagents`: Yes or No.
- `Rationale`: why subagents are or are not useful.
- `Critical path kept local`: what the lead keeps local to avoid blocking.
- `Worker scopes`: if workers are used, each worker has a separate scope.
- `Conflict risks`: files, modules, or contracts with conflict risk.

Use subagents when the work can be split into independent scopes with clear owners and worker-owned verification. Do not use subagents when the task is a sequential blocker, too small to split, or concentrated in files that are likely to conflict.

## Phase 3: Subagent Workflow

When the user requests the subagent workflow, the default flow uses:

- `leader`: analyze scope and create a plan only. The leader does not edit files.
- `coder`: implement the smallest safe change inside the assigned scope.
- `reviewer`: review the final diff, focusing on bugs, regressions, security, and tests.

The lead may add more workers when scopes are independent and conflict risk is low. Reviewer coverage must not be skipped when the subagent workflow is used.

Codex spawns subagents only when explicitly requested. Spawned agents inherit the active parent session sandbox and approval behavior, so the lead must keep permission assumptions in the task brief and final report.

Platform mapping:

- Codex uses custom agents in `.codex/agents/`.
- Claude Code uses project subagents in `.claude/agents/`.
- Cursor uses rules in `.cursor/rules/` and Agent or Background Agent sessions.

Codex custom agent files must stay narrow and define `name`, `description`, and `developer_instructions`. Optional model and sandbox fields are role defaults, not a guarantee that live parent-session overrides cannot apply.

## Phase 4: Task Brief And Assignment

Each worker assignment must include:

- Task goal.
- Delegation decision and rationale.
- Ownership scope.
- Files or directories the worker may edit.
- Files or directories the worker should avoid.
- Shared interfaces, contracts, or dependencies to preserve.
- Required verification.
- Handoff format.

Workers must understand they are not alone in the codebase. They must not revert changes outside their assigned scope.

## Phase 5: Worker Execution And Handoff

Workers stay inside the assigned scope. If completing the task requires changes outside that scope, the worker stops and reports the required scope change.

Every handoff must include:

- Scope completed.
- Files changed or evidence gathered.
- Commands run and outcomes.
- Verification results or skipped-verification reason.
- Risks or assumptions.
- Integration notes.
- Suggested next step.

## Phase 6: Integration

The lead reviews all handoffs before merging the work into the final result:

- Check whether each worker stayed inside the ownership scope.
- Check for conflicts with user changes or other worker changes.
- Check whether shared contracts remain intact.
- Resolve integration gaps with small, clear changes.

The lead must not trust handoffs blindly. If a worker reports passing verification, the lead still reviews the diff and runs the appropriate final verification before claiming completion.

## Phase 7: Verification

Verification depends on the task, but this flow prefers:

- Focused tests for the changed scope.
- Build or lint checks when code changes.
- Harness checks when docs, prompts, templates, plans, scripts, or the sample API change.
- Manual checks when the task affects UI, workflow, or endpoint behavior.

In this repository, run the quick harness with:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\Invoke-AgentHarness.ps1 -Mode Quick
```

Run endpoint probes with:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\Invoke-AgentHarness.ps1 -Mode Full -RunSampleApi
```

For docs-only changes that must exclude `src/`, do not use the sample API harness unless that scope restriction is lifted. Prefer static checks for the changed Markdown, Codex agent TOML metadata, skill metadata, and repository-local links.

Do not claim completion before verification runs. If verification cannot run, the final response must explain why and state what remains unverified.

## Phase 8: Final Response

The lead's final response should be concise and evidence-based:

- Files changed.
- Verification commands run and outcomes.
- Remaining risks, if any.
- Next step, if needed.

If subagents were used, the final response should summarize the handoffs and state how the lead reviewed and integrated them.

## Completion Gate

A task is complete only when:

- Implementation or research meets the success criteria.
- Worker handoffs, if any, have been reviewed by the lead.
- Integration has no known unresolved conflicts.
- Verification has run, or skipped verification has been clearly explained.
- The final response includes evidence instead of a generic completion claim.
