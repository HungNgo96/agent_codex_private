# Agent Team Coding Flow Overview

This project uses a lead-and-worker agent model. The lead agent owns context, coordination, integration, and final verification. Worker agents handle bounded independent tasks when parallel work is useful.

## 1. Lead Reads Context

The lead starts by reading the project context before assigning work:

- `AGENTS.md`
- relevant README or documentation
- current git status
- files related to the requested task

The lead decides whether the task should stay local or be split across workers.

## 2. Lead Defines Ownership

The lead creates clear, non-overlapping scopes only when delegation is useful.

Example ownership split:

- Worker A: backend API changes
- Worker B: frontend UI changes
- Worker C: focused tests, docs, or research

Each worker receives a bounded scope, expected output, and verification expectation.

## 3. Workers Execute In Parallel

Workers stay inside their assigned file, module, or research scope. They must preserve user work and avoid unrelated refactors.

Each worker handoff includes:

- scope completed
- files changed or evidence gathered
- commands run and outcomes
- verification results or skipped-verification reason
- known risks
- integration notes
- suggested next step

## 4. Lead Integrates Results

The lead reviews worker handoffs, integrates changes, resolves gaps, and ensures the final result follows existing project patterns.

The lead should not duplicate worker work unless integration requires it.

## 5. Lead Runs Final Verification

Completion requires verification. Depending on the project and change, this can include:

- tests
- lint
- typecheck
- build
- focused manual verification

If verification cannot be run, the lead reports exactly why and what remains unverified.

## 6. Lead Reports Completion

The final report includes:

- changed files
- verification evidence
- known risks
- remaining next steps, if any

## Rule Of Thumb

Use delegation only when work is independent, bounded, and useful in parallel. Keep urgent blocking work local when waiting for a worker would slow the critical path.
