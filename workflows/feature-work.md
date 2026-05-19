# Feature Work Workflow

Use this workflow for new behavior or meaningful changes to existing behavior.

## Steps

1. Lead reads project context and identifies existing patterns.
2. Lead writes a task brief with goals, non-goals, constraints, and success criteria.
3. Lead decides whether parallel work is useful.
4. Lead defines shared interfaces before dispatching workers.
5. Workers complete scoped tasks and return handoff notes.
6. Lead reviews handoffs and integrates changes.
7. Lead runs final verification.
8. Lead reports changed files, verification evidence, and residual risks.

## Delegation Guidance

Good slices include separate UI, API, tests, docs, or research tasks. Avoid assigning two workers to the same file set unless one is read-only.

Worker handoffs must include scope completed, files changed or evidence gathered, commands run and outcomes, focused worker-scope verification, known risks, integration notes, and a suggested next step before lead integration.

## Verification

Use the project's normal test, lint, build, and manual verification commands. If no commands are known, inspect project docs and package scripts before choosing a command.
