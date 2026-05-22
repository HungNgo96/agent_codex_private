---
name: agent-ai-feature-development
description: Use when implementing a feature or bug fix with the AgentTeams coding flow: plan scope, read module docs, assign leader/coder/reviewer responsibilities when requested, implement narrowly, verify, review, and report handoff evidence. Do not use for one-line edits, docs-only questions, or tasks that do not touch project code.
---

# Agent AI Feature Development

Use this skill for repeatable coding work where the user wants the AgentTeams flow applied to a feature, bug fix, or refactor.

## Inputs

- User request and acceptance criteria.
- Target module folder under `docs/modules/<module>/`, when available.
- Relevant workflow from `workflows/`.
- Relevant templates from `templates/`.
- Current git status.

## Workflow

1. Read `AGENTS.md` first.
2. Read the target module folder under `docs/modules/<module>/` if the task maps to a module.
3. Inspect the affected code before planning.
4. Define success criteria, editable scope, avoid scope, shared contracts, and verification.
5. Decide whether subagents are useful:
   - Use the main agent only for small, tightly coupled, or blocking work.
   - Use leader/coder/reviewer subagents only when the user requests subagents and the work splits into independent scopes.
6. Implement the smallest safe change that satisfies the request.
7. Update module `history.md` only for durable context that should help a future targeted task.
8. Run focused verification for the changed surface.
9. Review the final diff before reporting completion.

## Role Mapping

- Lead: owns decomposition, scope control, integration, final verification, and final report.
- Coder: owns the assigned implementation scope and focused verification.
- Reviewer: owns final diff review and reports findings first.

Subagents are optional execution helpers. They do not remove the lead agent's responsibility for integration, verification, and final reporting.

## Output

Report:

- Files changed.
- Commands run and outcomes.
- Verification evidence.
- Known risks or skipped checks.
- Reviewer result or review status.
- Suggested next step only when it materially advances the user's goal.
