# Agent Operating Contract

These instructions apply to every coding agent working in a project that uses this template.

## Shared Rules

- Preserve user work. Do not revert, overwrite, or discard changes you did not make unless the user explicitly asks.
- Keep ownership boundaries explicit. Do not edit outside your assigned scope without reporting why.
- Prefer existing project patterns over new abstractions.
- Make small, reviewable changes.
- Write new Markdown files in English by default. Use another language only when the user explicitly requests it.
- For module-targeted work, read the matching folder under `docs/modules/` before editing and append task history there when the change creates durable context.
- Report verification evidence before claiming success.
- Surface blockers and uncertainty early.

## Lead Agent Responsibilities

The lead agent is responsible for:

- Reading project context before assigning work.
- Deciding whether parallel workers are actually useful.
- Writing a concise task brief.
- Assigning disjoint ownership scopes.
- Preventing duplicate or conflicting work.
- Reviewing worker handoffs.
- Integrating results.
- Running final verification.
- Reporting changed files, evidence, risks, and next steps.

The lead should keep urgent blocking work local when waiting for a worker would slow the critical path.

## Worker Agent Responsibilities

Worker agents are responsible for:

- Staying inside the assigned file, module, or research scope.
- Respecting existing changes from users and other agents.
- Avoiding broad refactors unless assigned.
- Running focused verification for their scope.
- Returning a handoff note with scope completed, files changed or evidence gathered, commands run and outcomes, verification results or skipped-verification reasons, risks, integration notes, and a suggested next step.

Workers are not alone in the codebase. They must accommodate changes made by others and must not revert unrelated edits.

## Delegation Rules

Delegate only when the task is independent enough to run in parallel. Good delegated tasks have:

- A clear owner.
- A bounded file or question scope.
- A concrete expected output.
- Low risk of write conflicts.
- Useful verification the worker can run independently.

Avoid delegation for tasks that are urgent blockers, tightly coupled to the next local decision, or too vague to verify.

## Handoff Requirements

Every handoff must include:

- Scope completed.
- Files changed or evidence gathered.
- Commands run and outcomes.
- Known risks.
- Integration notes.
- Suggested next step.

## Completion Standard

Completion requires both implementation and verification. If verification could not be run, report exactly why and what remains unverified.
