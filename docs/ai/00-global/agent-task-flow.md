# Agent Task Mindset and File Flow

## Purpose

This document defines how an AI agent should think before, during, and after each task in this project. It helps the agent decide which documentation file to create under `docs/ai` so future tasks can reuse the context instead of rediscovering it.

## Core Mindset

- Understand the module before editing code.
- Preserve user work and avoid unrelated refactors.
- Keep the task scope small, explicit, and reviewable.
- Record decisions when they affect future behavior.
- Record plans when the task has meaningful implementation risk.
- Record reviews when the task investigates correctness, maintainability, security, performance, or business impact.
- Record changelogs when code behavior changes.
- Report verification evidence before claiming completion.

## Task Start Flow

1. Identify the affected module under `docs/ai`.
2. Read the module `README.md`, `rules.md`, and `decisions.md` when they exist.
3. Check `docs/ai/90-architecture` if the task touches shared structure, dependency injection, EF Core, migrations, or cross-module behavior.
4. Decide whether the task needs a documentation artifact.
5. Keep the artifact inside the affected module folder unless the task is global architecture or cross-cutting behavior.

## When to Create Files

### Create a Plan

Create `plans/YYYY-MM-DD-short-task-name.md` when the task:

- changes business logic;
- touches database schema, migrations, imports, exports, payroll, tax, email, contract, or employee data;
- has multiple implementation steps;
- has security, data integrity, or performance risk;
- needs explicit scope boundaries before coding.

The plan should include:

- goal;
- scope;
- current problem;
- relevant files;
- proposed flow;
- implementation steps;
- verification plan;
- out-of-scope items.

### Create a Review

Create `reviews/YYYY-MM-DD-short-review-name.md` when the task:

- asks for a review;
- investigates a bug or suspicious behavior;
- compares current code against expected business rules;
- evaluates performance, security, maintainability, or data integrity.

The review should include:

- review scope;
- files reviewed;
- findings ordered by severity;
- evidence or reasoning;
- suggested fixes;
- open questions;
- verification gaps.

### Create a Changelog

Create `changelogs/YYYY-MM-DD-short-task-name-changes.md` after implementation when:

- user-visible behavior changes;
- business rules change;
- validation changes;
- database read/write behavior changes;
- import/export output changes;
- email, tax, payroll, or contract behavior changes.

The changelog should include:

- summary;
- changed files;
- behavior before;
- behavior after;
- business rules applied;
- verification commands and outcomes;
- remaining risks.

### Create or Update Decisions

Update `decisions.md` when the task creates a durable rule or architecture choice, such as:

- choosing one service boundary over another;
- adding or changing a database invariant;
- changing a shared flow used by multiple modules;
- accepting a known tradeoff;
- deciding not to implement an expected alternative.

Decision entries should include:

- date;
- decision;
- reason;
- impact;
- alternatives considered when useful.

### Update Rules

Update `rules.md` when a repeated constraint becomes clear and should guide future agents, such as:

- a validation rule;
- an import/export constraint;
- a module ownership boundary;
- a field that must not be changed automatically;
- a performance rule.

Rules should be stable. Do not add one-time task notes to `rules.md`.

## File Placement

Use this structure inside each module:

```text
docs/ai/<module>/
  README.md
  rules.md
  decisions.md
  plans/
    YYYY-MM-DD-short-task-name.md
  reviews/
    YYYY-MM-DD-short-review-name.md
  changelogs/
    YYYY-MM-DD-short-task-name-changes.md
```

Use `docs/ai/00-global` for agent process, shared prompts, and coding rules.

Use `docs/ai/90-architecture` for architecture-wide decisions, shared flows, DI, EF Core, migrations, and cross-module structure.

## Task Completion Flow

Before finishing a task, the agent should check:

1. Was the affected module documented or updated when needed?
2. Was a plan created for risky or multi-step work?
3. Was a changelog created for behavior changes?
4. Were decisions or rules updated only when they are durable?
5. Were verification commands run?
6. If verification was skipped, is the reason documented?

## What Not to Document

Do not create extra files for:

- tiny text-only edits with no behavior impact;
- formatting-only changes;
- exploratory notes that are not useful later;
- temporary assumptions that were resolved during the task;
- copied source code.

## Agent Handoff Note

Each completed task should report:

- scope completed;
- files changed;
- commands run and outcomes;
- verification evidence;
- known risks;
- suggested next step.
