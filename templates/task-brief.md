# Task Brief

## Goal

State the outcome in one or two sentences.

## Non-Goals

List work that is intentionally out of scope.

## Context

Summarize relevant files, docs, constraints, and existing patterns.

## Constraints

- Editable scope:
- Avoid scope:
- Shared interfaces or dependencies:
- Time, compatibility, or verification constraints:

## Success Criteria

- Observable result 1.
- Observable result 2.
- Required verification.

## Delegation Decision

- Use subagents: Yes or No.
- Rationale:
- Critical path kept local:
- Worker scopes:
- Conflict risks:

## Team

- Lead:
- Worker:
- Worker:

## Ownership

Every assigned worker listed under Team must have a matching ownership row. Each worker row must state the worker-specific edit scope, avoid scope, shared interfaces or dependencies, and expected output.

| Owner | Edit Scope | Avoid Scope | Shared Interfaces / Dependencies | Output |
| --- | --- | --- | --- | --- |
| Lead | Coordination and integration | Worker-owned files unless needed for integration | Cross-scope contracts and final verification | Final verified result |
| Worker A | `path/or/module` and exact change area | Files, modules, or decisions outside assignment | APIs, schemas, fixtures, or docs shared with other scopes | Handoff with changed paths, verification, risks, and integration notes |
| Worker B | Research or verification scope | Code edits unless explicitly assigned | Inputs needed from Worker A or lead-owned integration | Evidence, commands run, findings, and next suggested step |

## Verification Plan

List exact commands, manual checks, or source checks expected before completion.
