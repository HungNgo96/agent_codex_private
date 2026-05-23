# Agent Teams Template

Reusable conventions for coordinating lead and worker agents on software projects, with Codex-first defaults and thin adapters for Cursor and Claude Code.

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
3. Read `docs/agent-ai-task-flow.md` and the relevant module folder under `docs/modules/`.
4. Read `docs/harness-coding-agent-flow.md` for long-running or high-ambiguity coding work.
5. Read `docs/codex-flow-audit.md` when running the template with Codex.
6. For repeatable feature or bug-fix work in Codex, use `$agent-ai-feature-development`.
7. Choose the relevant workflow from `workflows/`.
8. Have the lead create a task brief using `templates/task-brief.md`.
9. Dispatch workers only when work splits into bounded, independent scopes.
10. Collect handoffs with `templates/handoff-note.md`.
11. Run final integration and verification before reporting completion.

## Optimized Coding Plan

Use this lifecycle for Codex, Cursor, and Claude Code work:

1. Load context: read `AGENTS.md`, then explicitly read the relevant workflow, module docs, templates, and platform adapter before relying on them.
2. Define the task: write success criteria, editable scope, avoid scope, shared contracts, verification, and conflict risks in `templates/task-brief.md`.
3. Decide delegation: use one agent for small or sequential work; use `leader`, `coder`, and `reviewer` only when the task splits into bounded scopes.
4. Execute narrowly: the coder owns only the assigned scope and returns `templates/handoff-note.md` with evidence and verification.
5. Review before completion: the reviewer inspects the final diff for correctness, regressions, security, scope creep, and missing tests.
6. Integrate and verify: the main agent reviews handoffs, resolves gaps, runs final checks, and reports changed files, evidence, risks, and skipped checks.

The main agent remains accountable for coordination and final verification on every platform.

## Platform Setup

- Codex: use `AGENTS.md` plus the prompt, workflow, module-history, and template files in this repo.
- Cursor: use `.cursor/rules/agent-team-operating-contract.mdc`, which points Cursor Agent back to the shared contract.
- Claude Code: use `CLAUDE.md`, which points Claude Code back to the shared contract.

Codex reliably loads `AGENTS.md` as project guidance. Treat the rest of this repository as explicit task context: mention or read the relevant prompt, workflow, module folder, template, or doc before relying on it in a Codex run.

### Codex Subagents

Codex only spawns subagents when you explicitly ask it to. This template includes project-scoped custom agents in `.codex/agents/`:

- `leader`: analyze scope and create a plan only.
- `coder`: implement the smallest safe change inside the assigned scope.
- `reviewer`: review the final git diff.

Use `prompts/lead-agent.md` as the main coordination prompt, then ask for the subagent workflow when you want this three-agent flow. Codex handles spawning subagents, routing follow-up instructions, waiting for results, closing completed threads, and returning a consolidated response. Subagents inherit the active sandbox policy and approval controls from the parent session, so role files describe intent rather than permission guarantees.

For the Codex-specific audit and implementation flow, see `docs/codex-flow-audit.md`.

### Codex Skills

Codex scans repo-local skills from `.agents/skills/` when launched inside the repository. This template includes one intentionally narrow skill:

- `$agent-ai-feature-development`: repeatable feature or bug-fix workflow that ties together module docs, scope planning, optional leader/coder/reviewer delegation, implementation, verification, and final reporting.

Keep skills focused on one reusable job. Do not add repo-local skills for one-off conventions that already fit in `AGENTS.md`, module docs, workflows, or templates.

### Cursor Agent Flow

Cursor uses Project Rules in `.cursor/rules/` and can run asynchronous Background Agents. This template includes `.cursor/rules/agent-team-subagent-flow.mdc` to map the same `leader`, `coder`, and `reviewer` flow into Cursor.

Cursor does not load Codex `.codex/agents/*.toml` files or Claude Code `.claude/agents/*.md` files. Use Cursor Agent or Background Agents with the rule above, then have the main Cursor session coordinate the role outputs, review handoffs, run final verification, and summarize the result.

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
- `ARCHITECTURE.md`: system overview and knowledge-store entry point.
- `CLAUDE.md`: Claude Code project memory adapter for the shared contract.
- `.codex/agents/`: Codex custom project agents for the explicit subagent workflow.
- `.agents/skills/`: Codex repo-local skills for repeatable coding workflows.
- `.claude/agents/`: Claude Code project subagents for the explicit subagent workflow.
- `.cursor/rules/`: Cursor project rules adapter for the shared contract.
- `docs/modules/`: module ownership, rules, decisions, and task history.
- `docs/codex-flow-audit.md`: Codex-specific context-loading, subagent, skill, sandbox, and verification alignment notes.
- `docs/harness-coding-agent-flow.md`: harness layer for resumable working state, evaluator loops, observability, and entropy control.
- `prompts/`: paste-ready prompts for lead and worker sessions.
- `workflows/`: playbooks for common work types.
- `templates/`: task, handoff, review, and plan formats.

## Operating Model

The lead agent owns decomposition, assignment, integration, and final verification. Worker agents own bounded tasks and report back with scope completed, files changed or evidence gathered, commands run and outcomes, verification results or skipped-verification reasons, risks, integration notes, and a suggested next step.

Every worker must assume other agents may be editing the repo. Workers should stay inside their assigned scope, avoid reverting unrelated changes, and make handoffs clear enough for the lead to integrate without guessing.

## Module Task History

Use `docs/modules/<module>/` to keep durable context for targeted work. Each module can keep `README.md`, `rules.md`, `decisions.md`, and `history.md`. Before changing a module, read its folder; after meaningful work, append one short `history.md` entry.

## Verification Standard

Do not claim completion without evidence. Depending on the task, evidence can include tests, builds, linting, manual reproduction, browser checks, source citations, or local file references.

## Future Automation

Add orchestration scripts only after the workflow is proven manually and the automation has a clear owner.
