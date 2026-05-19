# Agent-Team Template Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a reusable Codex agent-team template with operating rules, team definitions, prompts, workflows, templates, examples, and future automation hooks.

**Architecture:** This is a documentation-first template repository. Root files explain the system and global agent contract, while focused subdirectories hold team profiles, reusable prompts, workflow playbooks, handoff templates, and examples. The `scripts/` directory is reserved for later orchestration helpers and contains only documentation in this version.

**Tech Stack:** Markdown documentation, PowerShell verification commands, no application runtime.

---

## File Structure

- Create `README.md`: template overview, quick start, usage model, repository map.
- Create `AGENTS.md`: global operating contract for lead and worker Codex agents.
- Create `teams/full-stack.md`: reusable team profile for multi-layer feature work.
- Create `teams/research-review.md`: reusable team profile for research and review tasks.
- Create `teams/debugging.md`: reusable team profile for systematic debugging.
- Create `prompts/lead-agent.md`: paste-ready prompt for the lead agent.
- Create `prompts/worker-agent.md`: paste-ready prompt for bounded worker agents.
- Create `workflows/feature-work.md`: playbook for feature implementation.
- Create `workflows/debugging.md`: playbook for reproducing, investigating, fixing, and verifying bugs.
- Create `workflows/code-review.md`: playbook for review-oriented work.
- Create `workflows/research.md`: playbook for source-backed research.
- Create `templates/task-brief.md`: reusable task brief format.
- Create `templates/handoff-note.md`: reusable worker handoff format.
- Create `templates/review-report.md`: reusable code review report format.
- Create `templates/implementation-plan.md`: reusable implementation plan format.
- Create `examples/full-stack-feature.md`: concrete example of lead/worker coordination.
- Create `scripts/README.md`: future automation boundary and non-goals.

### Task 1: Root Documentation

**Files:**
- Create: `README.md`
- Create: `AGENTS.md`

- [ ] **Step 1: Create `README.md`**

Write this content:

```markdown
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
```

- [ ] **Step 2: Create `AGENTS.md`**

Write this content:

```markdown
# Agent Operating Contract

These instructions apply to every Codex agent working in a project that uses this template.

## Shared Rules

- Preserve user work. Do not revert, overwrite, or discard changes you did not make unless the user explicitly asks.
- Keep ownership boundaries explicit. Do not edit outside your assigned scope without reporting why.
- Prefer existing project patterns over new abstractions.
- Make small, reviewable changes.
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
- Suggested next step, if any.

## Completion Standard

Completion requires both implementation and verification. If verification could not be run, report exactly why and what remains unverified.
```

- [ ] **Step 3: Verify root documentation exists**

Run:

```powershell
Get-ChildItem -Force README.md, AGENTS.md
```

Expected: both files are listed.

### Task 2: Team Definitions

**Files:**
- Create: `teams/full-stack.md`
- Create: `teams/research-review.md`
- Create: `teams/debugging.md`

- [ ] **Step 1: Create `teams/full-stack.md`**

Write this content:

```markdown
# Full-Stack Team

Use this team for features that span multiple layers and can be split into independent slices.

## Roles

### Lead Agent

Owns requirements, interfaces, task boundaries, integration, and final verification.

### Frontend Worker

Owns UI components, client state, browser behavior, accessibility checks, and frontend tests within the assigned scope.

### Backend Worker

Owns API endpoints, service logic, validation, persistence integration, and backend tests within the assigned scope.

### Test Worker

Owns focused regression tests, integration checks, and verification scripts. The test worker should coordinate with the lead before touching production code.

### Docs Worker

Owns user-facing docs, internal notes, migration instructions, or changelog entries when documentation is part of the task.

## Best Fit

- New product features.
- UI plus API changes.
- Data model changes with user-facing behavior.
- Work that can be split by directory or module.

## Avoid When

- The task is a one-file fix.
- The lead does not yet understand the required interfaces.
- Multiple workers would need to edit the same files at the same time.

## Coordination Contract

The lead must define shared interfaces before workers begin. Workers must report any interface change immediately in their handoff.
```

- [ ] **Step 2: Create `teams/research-review.md`**

Write this content:

```markdown
# Research Review Team

Use this team when several independent questions can be investigated in parallel.

## Roles

### Lead Agent

Defines research questions, source standards, output format, and synthesis criteria.

### Documentation Researcher

Checks official documentation, release notes, specifications, and primary sources.

### Code Explorer

Reads the local repository and reports evidence with file references. This role is read-only unless explicitly assigned implementation work.

### Risk Reviewer

Looks for contradictions, missing evidence, security concerns, behavioral regressions, and untested assumptions.

## Best Fit

- Library or framework behavior questions.
- Migration planning.
- Architecture comparison.
- Pre-implementation discovery.
- Independent codebase investigations.

## Source Standard

Prefer primary sources. For technical behavior, use official documentation, source code, standards, or directly reproducible local evidence.

## Output Standard

Every finding must include a source link or local file reference. Inferences must be labeled as inferences.
```

- [ ] **Step 3: Create `teams/debugging.md`**

Write this content:

```markdown
# Debugging Team

Use this team when a failure has multiple plausible causes that can be investigated independently.

## Roles

### Lead Agent

Owns reproduction, observed facts, hypothesis tracking, fix selection, and final verification.

### Reproduction Worker

Creates or confirms the smallest reliable reproduction. Reports exact steps, command output, and observed failure.

### Code Path Worker

Traces the relevant implementation path and identifies where behavior diverges from expectations.

### Environment Worker

Checks configuration, dependency versions, runtime settings, generated artifacts, and external service assumptions.

### Verification Worker

Runs focused checks after a candidate fix and confirms the original failure no longer reproduces.

## Best Fit

- Flaky tests.
- Production-only failures.
- Configuration-sensitive bugs.
- Performance regressions with multiple possible bottlenecks.

## Debugging Rules

Do not patch before reproducing or gathering enough evidence. The lead should keep a visible list of facts and hypotheses so workers do not duplicate effort.
```

- [ ] **Step 4: Verify team files exist**

Run:

```powershell
Get-ChildItem teams -File
```

Expected: `debugging.md`, `full-stack.md`, and `research-review.md` are listed.

### Task 3: Agent Prompts

**Files:**
- Create: `prompts/lead-agent.md`
- Create: `prompts/worker-agent.md`

- [ ] **Step 1: Create `prompts/lead-agent.md`**

Write this content:

```markdown
# Lead Agent Prompt

You are the lead agent for this task.

First, read `AGENTS.md` and the relevant team or workflow document. Then inspect the project context before assigning work.

Your responsibilities:

- Understand the user's goal and success criteria.
- Decide whether parallel workers are useful.
- Keep urgent blocking work local.
- Create a concise task brief.
- Assign workers only bounded, independent tasks.
- Define exact ownership scopes for every worker.
- Tell workers they are not alone in the codebase and must not revert others' work.
- Review all worker handoffs before integration.
- Run final verification.
- Report changed files, verification evidence, unresolved risks, and next steps.

When dispatching a worker, include:

- Task goal.
- Ownership scope.
- Files or directories they may edit.
- Files or directories they should avoid.
- Shared interfaces, contracts, or dependencies they must preserve.
- Required verification.
- Handoff format.

Do not claim completion until verification has run or the reason it could not run is clear.
```

- [ ] **Step 2: Create `prompts/worker-agent.md`**

Write this content:

```markdown
# Worker Agent Prompt

You are a worker agent assigned a bounded task by the lead.

Read `AGENTS.md` before starting. You are not alone in the codebase. Other agents or the user may be changing nearby files. Do not revert edits you did not make.

Your responsibilities:

- Stay inside the assigned ownership scope.
- Ask for clarification only if the task cannot be completed safely.
- Prefer existing project patterns.
- Keep changes focused.
- Run the verification requested by the lead.
- Report any skipped verification and why.

Do not edit outside your assigned files or modules unless the task is impossible without doing so. If that happens, stop and report the required scope change.

Return a handoff with:

- Scope completed.
- Files changed or evidence gathered.
- Commands run and outcomes.
- Verification results or skipped-verification reason.
- Risks or assumptions.
- Integration notes.
- Suggested next step.
```

- [ ] **Step 3: Verify prompt files exist**

Run:

```powershell
Get-ChildItem prompts -File
```

Expected: `lead-agent.md` and `worker-agent.md` are listed.

### Task 4: Workflow Playbooks

**Files:**
- Create: `workflows/feature-work.md`
- Create: `workflows/debugging.md`
- Create: `workflows/code-review.md`
- Create: `workflows/research.md`

- [ ] **Step 1: Create `workflows/feature-work.md`**

Write this content:

```markdown
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

## Verification

Use the project's normal test, lint, build, and manual verification commands. If no commands are known, inspect project docs and package scripts before choosing a command.
```

- [ ] **Step 2: Create `workflows/debugging.md`**

Write this content:

```markdown
# Debugging Workflow

Use this workflow when something fails or behaves unexpectedly.

## Steps

1. Lead reproduces the issue or records why reproduction is not available.
2. Lead captures observed facts.
3. Lead lists competing hypotheses.
4. Lead assigns independent hypotheses to workers when useful.
5. Workers report evidence, not guesses.
6. Lead selects the smallest fix supported by evidence.
7. Lead verifies the original failure is resolved.
8. Lead adds or updates regression coverage when practical.

## Evidence Rules

Do not treat a plausible explanation as a fact. Facts come from command output, logs, source code, tests, documentation, or direct reproduction.

## Verification

The final report must include the reproduction command or manual steps and the passing verification after the fix.
```

- [ ] **Step 3: Create `workflows/code-review.md`**

Write this content:

```markdown
# Code Review Workflow

Use this workflow when reviewing proposed changes.

## Steps

1. Lead gathers changed files and relevant context.
2. Reviewer inspects correctness, regressions, security, maintainability, and missing tests.
3. Findings are reported first, ordered by severity.
4. Each finding includes a file and line reference when possible.
5. Reviewer separates confirmed issues from questions and assumptions.
6. Lead summarizes residual risk and verification gaps.

## Finding Format

Use this format:

```text
Severity: High | Medium | Low
File: path/to/file.ext:line
Issue: What is wrong.
Impact: What can break.
Recommendation: Concrete fix or next check.
```

## Review Standard

Do not pad the review with style preferences. Report issues that matter for correctness, safety, user behavior, or maintainability.
```

- [ ] **Step 4: Create `workflows/research.md`**

Write this content:

```markdown
# Research Workflow

Use this workflow for source-backed investigation.

## Steps

1. Lead defines exact research questions.
2. Lead defines acceptable sources.
3. Workers investigate independent questions or source areas.
4. Workers cite sources or local file references.
5. Lead compares findings and resolves contradictions.
6. Lead labels uncertainty and inferences.
7. Lead delivers a concise synthesis with links or file references.

## Source Priority

Prefer official documentation, standards, source code, release notes, and direct local evidence. Use secondary sources only when primary sources are unavailable or when they provide context clearly labeled as secondary.

## Output Standard

Every answer should make it clear what is verified, what is inferred, and what remains uncertain.
```

- [ ] **Step 5: Verify workflow files exist**

Run:

```powershell
Get-ChildItem workflows -File
```

Expected: `code-review.md`, `debugging.md`, `feature-work.md`, and `research.md` are listed.

### Task 5: Reusable Templates

**Files:**
- Create: `templates/task-brief.md`
- Create: `templates/handoff-note.md`
- Create: `templates/review-report.md`
- Create: `templates/implementation-plan.md`

- [ ] **Step 1: Create `templates/task-brief.md`**

Write this content:

```markdown
# Task Brief

## Goal

State the outcome in one or two sentences.

## Non-Goals

List work that is intentionally out of scope.

## Context

Summarize relevant files, docs, constraints, and existing patterns.

## Success Criteria

- Observable result 1.
- Observable result 2.
- Required verification.

## Team

- Lead:
- Worker:
- Worker:

## Ownership

| Owner | Scope | Output |
| --- | --- | --- |
| Lead | Coordination and integration | Final verified result |

## Verification Plan

List exact commands, manual checks, or source checks expected before completion.
```

- [ ] **Step 2: Create `templates/handoff-note.md`**

Write this content:

```markdown
# Handoff Note

## Assigned Scope

Describe the scope assigned by the lead.

## Completed Work

- Completed item 1.
- Completed item 2.

## Files Changed

- `path/to/file`

## Evidence Gathered

- Source, command output, or finding.

## Commands Run

| Command or Check | Outcome |
| --- | --- |
| `command here` | Passed, failed, or not run with reason |

## Verification

| Command or Check | Result |
| --- | --- |
| `command here` | Passed, failed, or not run with reason |

## Risks and Assumptions

- Risk or assumption 1.

## Integration Notes

Explain what the lead needs to know before merging or building on this work.

## Suggested Next Step

State the next action for the lead or the next worker.
```

- [ ] **Step 3: Create `templates/review-report.md`**

Write this content:

```markdown
# Review Report

## Findings

### Finding 1

Severity: High | Medium | Low
File: `path/to/file:line`
Issue: Describe the problem.
Impact: Describe what can break.
Recommendation: Describe the concrete fix or next check.

## Questions

- Question or assumption that needs confirmation.

## Verification Gaps

- Check that was not run or coverage that is missing.

## Summary

Briefly summarize overall risk after the findings.
```

- [ ] **Step 4: Create `templates/implementation-plan.md`**

Write this content:

```markdown
# Implementation Plan

## Goal

State what this plan builds or changes.

## Files

- Create:
- Modify:
- Test:

## Tasks

- [ ] Task 1: Describe a small, verifiable step.
- [ ] Task 2: Describe a small, verifiable step.

## Verification

- Command or manual check:
- Expected result:

## Handoff

Record scope completed, files changed or evidence gathered, commands run and outcomes, verification evidence, risks, integration notes, and suggested next step.
```

- [ ] **Step 5: Verify template files exist**

Run:

```powershell
Get-ChildItem templates -File
```

Expected: `handoff-note.md`, `implementation-plan.md`, `review-report.md`, and `task-brief.md` are listed.

### Task 6: Example and Automation Boundary

**Files:**
- Create: `examples/full-stack-feature.md`
- Create: `scripts/README.md`

- [ ] **Step 1: Create `examples/full-stack-feature.md`**

Write this content:

```markdown
# Example: Full-Stack Feature Team

## Scenario

Add a comments feature to an existing web application.

## Lead Brief

Goal: Users can view and submit comments on an article page.

Non-goals: Moderation, notifications, rich text, and admin tools are out of scope.

Success criteria:

- Article pages show existing comments.
- Signed-in users can submit a comment.
- Empty comments are rejected.
- Tests cover API validation and the primary UI flow.

## Team Assignment

| Agent | Ownership | Output |
| --- | --- | --- |
| Lead | Contracts, integration, final verification | Working feature and final report |
| Frontend Worker | Article comments UI files only | UI implementation and frontend checks |
| Backend Worker | Comments API and persistence files only | API implementation and backend tests |
| Test Worker | Test files only unless lead approves scope expansion | Regression coverage and test results |

## Worker Dispatch Example

```text
You are the Frontend Worker. You are not alone in the codebase; do not revert others' changes.

Goal: Add the comments UI to the article page.
Ownership: `src/article/` UI components and related frontend tests.
Avoid: API handlers, database files, unrelated layout refactors.
Shared interfaces/dependencies: Comment read model, create-comment response, and server-derived author identity.
Verification: Run relevant scoped frontend checks. If no check can be run, state the skipped-verification reason explicitly.
Handoff: Report scope completed, files changed or evidence gathered, commands run, results, risks/assumptions, integration notes, and suggested next step.
```

## Integration Notes

The lead should define the comment data shape before dispatching workers:

```json
{
  "id": "string",
  "articleId": "string",
  "authorName": "string",
  "body": "string",
  "createdAt": "ISO-8601 string"
}
```

The lead integrates worker handoffs, resolves contract mismatches, and runs the final project verification commands.
```

- [ ] **Step 2: Create `scripts/README.md`**

Write this content:

```markdown
# Scripts

This directory is reserved for future automation.

The first version of this template is documentation-only. Add scripts here after the manual workflow is proven.

Potential future helpers:

- Generate task briefs from templates.
- Create timestamped handoff notes.
- Launch multiple agent sessions.
- Aggregate worker handoffs.
- Validate required sections in handoff documents.

Do not add orchestration scripts until the repository has a clear, repeated manual workflow to automate.
```

- [ ] **Step 3: Verify example and script docs exist**

Run:

```powershell
Get-ChildItem examples,scripts -File
```

Expected: `examples/full-stack-feature.md` and `scripts/README.md` are listed.

### Task 7: Final Verification

**Files:**
- Read: all created Markdown files.

- [ ] **Step 1: List all Markdown files**

Run:

```powershell
Get-ChildItem -Recurse -File -Filter *.md | Sort-Object FullName | Select-Object -ExpandProperty FullName
```

Expected: the list includes the design spec, this plan, and all template files.

- [ ] **Step 2: Search for unfinished-marker text**

Run:

```powershell
Select-String -Path (Get-ChildItem -Recurse -File -Filter *.md).FullName -Pattern 'T[ ]BD|TO[ ]DO|implement[ ]later|fill[ ]in[ ]details'
```

Expected: no matches.

- [ ] **Step 3: Confirm core directories exist**

Run:

```powershell
Get-ChildItem -Directory | Select-Object -ExpandProperty Name
```

Expected: includes `docs`, `examples`, `prompts`, `scripts`, `teams`, `templates`, and `workflows`.

- [ ] **Step 4: Final report**

Report:

- Files created.
- Verification commands run.
- Whether the workspace is a git repository.
- Any skipped commit due to missing git repository.

## Self-Review

- Spec coverage: all approved files and concepts are covered by Tasks 1-7.
- Unfinished-marker scan: the plan contains no banned unfinished-work markers.
- Type consistency: this plan creates Markdown files only; paths and directory names match the design spec.
