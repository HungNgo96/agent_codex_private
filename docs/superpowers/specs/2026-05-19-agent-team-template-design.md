# Codex Agent-Team Template Design

Date: 2026-05-19
Status: Approved design, pending implementation plan

## Goal

Create a reusable template repository for running coordinated Codex agent teams on software projects. The template should provide clear operating conventions, reusable role prompts, workflow playbooks, handoff formats, and examples that can be copied into a real project.

This template will not implement a custom multi-agent runtime in the first version. Codex already supports lead-agent delegation through sub-agents; the template should make that process predictable by defining roles, ownership boundaries, task handoffs, and verification expectations.

## Non-Goals

- No custom process supervisor or terminal multiplexer integration in the initial version.
- No provider-specific Claude Agent Teams configuration.
- No automation that spawns agents automatically.
- No project-specific application code.

## Repository Structure

```text
.
|-- README.md
|-- AGENTS.md
|-- teams/
|   |-- full-stack.md
|   |-- research-review.md
|   `-- debugging.md
|-- prompts/
|   |-- lead-agent.md
|   `-- worker-agent.md
|-- workflows/
|   |-- feature-work.md
|   |-- debugging.md
|   |-- code-review.md
|   `-- research.md
|-- templates/
|   |-- task-brief.md
|   |-- handoff-note.md
|   |-- review-report.md
|   `-- implementation-plan.md
|-- examples/
|   `-- full-stack-feature.md
`-- scripts/
    `-- README.md
```

## Core Concepts

### Lead Agent

The lead agent owns planning, decomposition, assignment, coordination, integration, and final verification. The lead decides whether a task benefits from parallel workers, assigns disjoint ownership boundaries, and reviews returned work before presenting results to the user.

### Worker Agents

Worker agents receive bounded tasks with explicit ownership. A worker should edit only the assigned files or modules, avoid reverting changes made by others, and report changed paths, verification performed, unresolved risks, and integration notes.

### Ownership Boundaries

Every delegated task must name the worker's scope. Examples include a directory, a file set, a documentation area, a test suite, or a read-only research question. Workers should assume other agents may be editing nearby files and should accommodate existing changes rather than overwrite them.

### Handoffs

Every handoff should include:

- Scope completed.
- Files changed or evidence gathered.
- Commands run and outcomes.
- Verification results or skipped-verification reasons.
- Known risks.
- Integration notes.
- Suggested next step.

### Verification

The template should require evidence before completion claims. Verification can include tests, builds, linting, manual inspection, browser checks, or source-cited research depending on the workflow.

## Team Definitions

### Full-Stack Team

Use for features that span multiple layers. Suggested roles:

- Lead Agent: task breakdown, contracts, integration.
- Frontend Worker: UI and client behavior.
- Backend Worker: API, services, persistence integration.
- Test Worker: targeted tests and regression checks.
- Docs Worker: user-facing and internal documentation when needed.

### Research Review Team

Use when multiple independent questions can be answered in parallel. Suggested roles:

- Lead Agent: research questions, source quality rules, synthesis.
- Docs Researcher: official docs and version behavior.
- Code Explorer: repository evidence.
- Reviewer: risks, contradictions, and missing verification.

### Debugging Team

Use when a failure has multiple plausible causes. Suggested roles:

- Lead Agent: reproduce, hypothesis tracking, final fix selection.
- Reproduction Worker: creates or confirms minimal failing case.
- Code Path Worker: traces implementation path.
- Environment Worker: checks configuration, dependency, and runtime factors.
- Verification Worker: confirms the fix and regression coverage.

## Workflow Design

### Feature Work

1. Lead reads project context and writes a concise task brief.
2. Lead identifies independent slices and assigns workers only where parallel work is useful.
3. Workers complete scoped tasks and produce handoff notes.
4. Lead reviews changes, resolves integration issues, and runs final verification.
5. Lead reports changed files, verification evidence, and remaining risks.

### Debugging

1. Lead reproduces or confirms the failure.
2. Lead records observed facts and candidate hypotheses.
3. Workers investigate independent hypotheses.
4. Lead chooses the smallest supported fix.
5. Verification proves the original failure is resolved.

### Code Review

1. Lead gathers changed files and relevant context.
2. Review worker inspects correctness, security, regressions, and missing tests.
3. Findings are reported first, ordered by severity, with file and line references.
4. Lead summarizes residual risk and test gaps.

### Research

1. Lead defines exact questions and source standards.
2. Workers investigate independent sources or code areas.
3. Lead synthesizes with citations or local file references.
4. Contradictions and confidence levels are called out explicitly.

## Future Automation

The `scripts/` directory is reserved for optional launch and orchestration helpers. Future scripts may:

- Generate task briefs from templates.
- Create timestamped handoff files.
- Launch multiple agent sessions.
- Aggregate worker reports.
- Validate that required sections exist in handoff documents.

The initial documentation structure should remain useful without these scripts.

## Success Criteria

- A developer can copy the template into a project and understand how to run a lead/worker Codex workflow.
- Lead and worker prompts are ready to paste into Codex sessions.
- Team definitions cover feature work, research review, and debugging.
- Workflows include task ownership, handoff, and verification expectations.
- The template leaves room for future automation without requiring it.

## Self-Review

- No unfinished-work markers remain.
- Scope is limited to a reusable template, not runtime implementation.
- The structure matches the approved hybrid approach.
- Future automation is explicitly deferred to `scripts/` and does not block initial value.
