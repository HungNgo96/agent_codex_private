# Claude Code Agent Team Instructions

This project uses the shared agent-team template in this repository.

## Required Context

Before making changes, read:

- `AGENTS.md` for the shared operating contract.
- `prompts/lead-agent.md` when acting as the lead agent.
- `prompts/worker-agent.md` when acting as a bounded worker.
- The relevant file in `workflows/` for the task type.
- The relevant module folder under `docs/modules/` when the task targets a module.

## Operating Rules

- Preserve user work. Do not revert, overwrite, or discard changes you did not make unless explicitly asked.
- Keep ownership boundaries explicit.
- Prefer existing project patterns over new abstractions.
- Make small, reviewable changes.
- Report verification evidence before claiming success.
- Surface blockers and uncertainty early.

## Lead Agent Use

When acting as lead:

1. Read project context before assigning work.
2. Create a task brief using `templates/task-brief.md` with success criteria, editable scope, avoid scope, shared contracts, verification, and conflict risks.
3. Decide whether parallel workers are actually useful.
4. Keep urgent blocking work local.
5. Assign disjoint ownership scopes.
6. Review worker handoffs.
7. Integrate results.
8. Run final verification.
9. Report changed files, evidence, risks, skipped checks, and next steps.

## Worker Agent Use

When acting as a worker:

1. Stay inside the assigned file, module, or research scope.
2. Respect existing changes from users and other agents.
3. Avoid broad refactors unless assigned.
4. Run focused verification for the assigned scope.
5. Return a handoff using `templates/handoff-note.md`.

Workers are not alone in the codebase. They must accommodate changes made by others and must not revert unrelated edits.

## Claude Code Subagents

This project includes Claude Code project subagents in `.claude/agents/`:

- `leader`: analyze scope and create a plan only.
- `coder`: implement the smallest safe change inside the assigned scope.
- `reviewer`: review the final git diff.

Use these subagents when the user explicitly asks for the subagent workflow. The main Claude Code session coordinates the subagents, waits for their results, reviews their handoffs, and summarizes the final result. Do not skip review.

The optimized coding plan is the same as Codex and Cursor: load context, write the task brief, delegate only bounded scopes, collect handoffs, review the final diff, run final verification, and report evidence.

## Completion Standard

Completion requires implementation and verification. If verification could not be run, report exactly why and what remains unverified.
