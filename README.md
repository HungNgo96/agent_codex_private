# agent_codex_private
# Agent Teams Template

Reusable conventions for coordinating lead and worker agents on software projects, with adapters for Codex, Cursor, and Claude Code.

This template is not a custom multi-agent runtime. It gives you the operating docs, prompts, workflows, and handoff formats needed to run agent-team work using your coding agent environment.

## When To Use This

Use this template when work can be split into independent slices, such as:

- Full-stack features with separate frontend, backend, test, and docs work.
- Research tasks with independent questions or source areas.
- Debugging tasks with multiple plausible causes.
- Code reviews where one agent can inspect while another gathers context.

Use a single agent when the task is small, tightly coupled, or blocked on one sequential discovery path.

## Quick Start

1. Copy this template into the root of a project.
2. Read `AGENTS.md` to understand the shared operating contract.
3. Pick a team from `teams/`.
4. Start a lead session with `prompts/lead-agent.md`.
5. Have the lead create a task brief using `templates/task-brief.md`.
6. Dispatch workers with `prompts/worker-agent.md` and a bounded ownership scope.
7. Collect handoffs with `templates/handoff-note.md`.
8. Run final integration and verification before reporting completion.

## Platform Setup

- Codex: use `AGENTS.md` plus the prompt, workflow, team, and template files in this repo.
- Cursor: use `.cursor/rules/agent-team-operating-contract.mdc`, which points Cursor Agent back to the shared contract.
- Claude Code: use `CLAUDE.md`, which points Claude Code back to the shared contract.

### Codex Subagents

Codex only spawns subagents when you explicitly ask it to. This template includes project-scoped custom agents in `.codex/agents/`:

- `hermes`: memory-aware, skill-aware coordinator inspired by Hermes Agent.
- `leader`: analyze scope and create a plan only.
- `coder`: implement the smallest safe change inside the assigned scope.
- `reviewer`: review the final git diff.

Use `prompts/lead-agent.md` as the main coordination prompt, then ask for the subagent workflow when you want this three-agent flow. Codex handles spawning subagents, routing follow-up instructions, waiting for results, closing completed threads, and returning a consolidated response. Subagents inherit the active sandbox policy and approval controls from the parent session.

For the Hermes-inspired Codex flow, see `docs/hermes-codex-flow.md`. This template borrows Hermes Agent ideas such as memory-aware work, skill-aware execution, reflection, and subagent delegation, but it does not install or run the Hermes runtime.

### Cursor Agent Flow

Cursor uses Project Rules in `.cursor/rules/` and can run asynchronous Background Agents. This template includes `.cursor/rules/agent-team-subagent-flow.mdc` to map the same `leader`, `coder`, and `reviewer` flow into Cursor.

Cursor does not load Codex `.codex/agents/*.toml` files or Claude Code `.claude/agents/*.md` files. Use Cursor Agent or Background Agents with the rule above, then have the main Cursor session coordinate the role outputs and final summary.

### Claude Code Subagents

Claude Code project subagents live in `.claude/agents/` as Markdown files with YAML frontmatter. This template includes:

- `.claude/agents/leader.md`
- `.claude/agents/coder.md`
- `.claude/agents/reviewer.md`

Use Claude Code's `/agents` command to inspect or edit project subagents, or invoke them explicitly by name in your request.

Example request:

```text
Use subagent workflow.
Spawn 3 agents:
1. leader: analyze scope and create plan only
2. coder: implement the smallest safe change
3. reviewer: review final git diff

Main agent must coordinate them and summarize final result.
Do not skip review.
```

## Repository Map

- `AGENTS.md`: shared rules for every agent working in the repo.
- `CLAUDE.md`: Claude Code project memory adapter for the shared contract.
- `.codex/agents/`: Codex custom project agents for the explicit subagent workflow.
- `.claude/agents/`: Claude Code project subagents for the explicit subagent workflow.
- `.cursor/rules/`: Cursor project rules adapter for the shared contract.
- `docs/hermes-codex-flow.md`: Hermes-inspired Codex workflow notes.
- `teams/`: reusable team compositions.
- `prompts/`: paste-ready prompts for lead and worker sessions.
- `workflows/`: playbooks for common work types.
- `templates/`: task, handoff, review, and plan formats.
- `examples/`: complete examples of agent-team usage.
- `scripts/`: local harness and optional future orchestration helpers.

## Operating Model

The lead agent owns decomposition, assignment, integration, and final verification. Worker agents own bounded tasks and report back with scope completed, files changed or evidence gathered, commands run and outcomes, verification results or skipped-verification reasons, risks, integration notes, and a suggested next step.

Every worker must assume other agents may be editing the repo. Workers should stay inside their assigned scope, avoid reverting unrelated changes, and make handoffs clear enough for the lead to integrate without guessing.

## Verification Standard

Do not claim completion without evidence. Depending on the task, evidence can include tests, builds, linting, manual reproduction, browser checks, source citations, or local file references.

## Local Harness

Run the local QA harness to validate the agent-team docs, templates, implementation plans, and sample API:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\Invoke-AgentHarness.ps1 -Mode Quick
```

For full verification with endpoint probes:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\Invoke-AgentHarness.ps1 -Mode Full -RunSampleApi
```

See `docs/harness-engineering.md` for the check list and report format.

## Future Automation

Add orchestration scripts only after the workflow is proven manually and the harness can validate the expected contracts.
