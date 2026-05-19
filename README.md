# agent_codex_private
# Codex Agent Teams Template

Reusable conventions for coordinating Codex lead and worker agents on software projects.

This template is not a custom multi-agent runtime. It gives you the operating docs, prompts, workflows, and handoff formats needed to run agent-team work using Codex delegation.

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
4. Start a lead Codex session with `prompts/lead-agent.md`.
5. Have the lead create a task brief using `templates/task-brief.md`.
6. Dispatch workers with `prompts/worker-agent.md` and a bounded ownership scope.
7. Collect handoffs with `templates/handoff-note.md`.
8. Run final integration and verification before reporting completion.

## Repository Map

- `AGENTS.md`: shared rules for every agent working in the repo.
- `teams/`: reusable team compositions.
- `prompts/`: paste-ready prompts for lead and worker sessions.
- `workflows/`: playbooks for common work types.
- `templates/`: task, handoff, review, and plan formats.
- `examples/`: complete examples of agent-team usage.
- `scripts/`: reserved for optional future orchestration helpers.

## Operating Model

The lead agent owns decomposition, assignment, integration, and final verification. Worker agents own bounded tasks and report back with scope completed, files changed or evidence gathered, commands run and outcomes, verification results or skipped-verification reasons, risks, integration notes, and a suggested next step.

Every worker must assume other agents may be editing the repo. Workers should stay inside their assigned scope, avoid reverting unrelated changes, and make handoffs clear enough for the lead to integrate without guessing.

## Verification Standard

Do not claim completion without evidence. Depending on the task, evidence can include tests, builds, linting, manual reproduction, browser checks, source citations, or local file references.

## Future Automation

The initial template is intentionally documentation-only. Add scripts later only after the workflow is proven manually.
