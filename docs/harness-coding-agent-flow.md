# Harness Coding Agent Flow

This document translates current harness-engineering guidance into the AgentTeams coding flow. It is a design guide for making coding agents reliable without turning this repository into a custom runtime.

## Reference Principles

The flow is based on these external references:

- OpenAI, ["Harness engineering: leveraging Codex in an agent-first world"](https://openai.com/index/harness-engineering/): repository-local knowledge, agent-legible application state, architectural constraints, feedback loops, and continuous cleanup.
- Anthropic, ["Effective harnesses for long-running agents"](https://www.anthropic.com/engineering/effective-harnesses-for-long-running-agents): initializer sessions, feature lists, resumable progress files, incremental work, and end-to-end testing.
- Anthropic, ["Harness design for long-running application development"](https://www.anthropic.com/engineering/harness-design-long-running-apps): planner, builder, and evaluator loops for tasks that exceed a single model's reliable solo performance.
- [`walkinglabs/awesome-harness-engineering`](https://github.com/walkinglabs/awesome-harness-engineering): curated references for context, memory, constraints, evaluation, observability, runtime control, and safe autonomy.

## Harness Layers

AgentTeams should treat the harness as five layers around the model:

1. Context map: short entry points such as `AGENTS.md`, `ARCHITECTURE.md`, `docs/agent-ai-task-flow.md`, and module docs that tell the agent where to look next.
2. Working state: task briefs, handoff notes, module history, active plans, and verification results that let future sessions resume without guessing.
3. Execution surface: standard repo tools, build scripts, tests, browser automation, API checks, and local app boot commands that the agent can run directly.
4. Guardrails: ownership scopes, avoid scopes, architecture rules, review requirements, sandbox assumptions, and mechanical checks.
5. Feedback loops: tests, linting, E2E checks, screenshots, logs, metrics, review findings, and cleanup tasks that turn failures into durable rules or tooling.

## End-to-End Harness Flow

```text
User goal
  |
  v
Lead loads context map
  |
  v
Lead writes task brief and working-state ledger
  |
  v
Initializer step, if the repo or task lacks runnable state
  |
  v
Planner splits work only when useful
  |
  +--> Single-session implementation
  |
  +--> Bounded workers: coder, researcher, reviewer, evaluator
  |
  v
Agent executes one narrow slice
  |
  v
Agent verifies with executable checks
  |
  v
Agent records handoff and progress state
  |
  v
Lead integrates, reviews, and reruns final verification
  |
  v
Durable updates: docs, rules, tests, history, or cleanup follow-up
```

## Required Artifacts

Use these artifacts when the task is more than a one-shot edit:

| Artifact | Purpose | Existing Location |
| --- | --- | --- |
| Operating contract | Stable rules for all agents | `AGENTS.md` |
| Context map | Where to find architecture, workflows, and module context | `ARCHITECTURE.md`, `docs/modules/` |
| Task brief | Goal, scope, non-goals, delegation, verification, and conflict risks | `templates/task-brief.md` |
| Progress ledger | Durable state for future sessions | `docs/modules/<module>/history.md` or a task-specific plan |
| Feature or test ledger | Observable acceptance checks and pass/fail status | Add only when a task has many independent acceptance cases |
| Handoff note | Worker evidence and integration notes | `templates/handoff-note.md` |
| Review report | Final risk-focused inspection | `templates/review-report.md` |

Do not create all artifacts by default. Create the smallest set that makes the task resumable, verifiable, and reviewable.

## Initializer Step

Use an initializer step when the task or repository lacks enough executable state for later agents to work safely. The initializer does not implement feature code unless the setup itself is the task.

The initializer should produce:

- A concise environment note: prerequisites, commands, ports, required services, and known missing setup.
- A runnable verification baseline: the smallest command set that proves the current state.
- A feature or acceptance ledger when the request contains many behaviors.
- A first task brief or implementation plan that future coding sessions can continue.

Prefer existing files such as module docs and templates. Add new task-specific files only when they will outlive the current session.

## Incremental Execution Rules

Long-running coding work should move in small slices:

- Pick one acceptance item or failure at a time.
- Keep the critical path local when delegation would slow discovery.
- Dispatch workers only with disjoint ownership scopes.
- After each slice, run focused verification and record the result.
- Commit or checkpoint only when the user requested git operations or the project workflow explicitly allows it.
- If the agent discovers missing harness support, add a narrow rule, test, script, or doc entry instead of relying on memory.

## Evaluator Loop

Use a separate evaluator or reviewer when the output quality depends on subjective product judgment, end-to-end behavior, UI correctness, security, or cross-module integration.

The evaluator should inspect:

- Whether acceptance criteria are actually observable in the product or API.
- Whether the agent used real checks instead of assuming success.
- Whether new patterns are agent-legible and mechanically enforceable.
- Whether failures should become tests, docs, or cleanup items.

For small or tightly coupled tasks, the lead can perform the evaluator role locally.

## Observability And App Legibility

Agents should be able to inspect the system they are changing. When a project grows beyond simple unit tests, expose these signals through standard commands or documented local tools:

- App startup and health checks.
- Browser or API flows for user-facing behavior.
- Logs with enough structure to trace failures.
- Metrics or traces for performance-sensitive work.
- Screenshots or recorded evidence for visual changes.

If an agent cannot observe a behavior, that behavior is not yet harnessed.

## Cleanup And Entropy Control

Agent output drifts toward existing patterns, including weak ones. Convert repeated review feedback into durable enforcement:

- Promote recurring comments into `AGENTS.md`, module rules, tests, linters, or templates.
- Keep `AGENTS.md` short; add deeper guidance to module docs or workflow docs.
- Add cleanup tasks for stale docs, dead patterns, and weak verification rather than broad refactors during feature work.
- Prefer mechanical checks for architecture and quality rules whenever practical.

## Fit With AgentTeams

The AgentTeams role model stays simple:

- Lead: owns context loading, task brief, delegation, integration, final verification, and durable updates.
- Coder: owns a bounded implementation slice and focused verification.
- Reviewer or evaluator: inspects diff, behavior, tests, and harness gaps.
- Researcher: gathers source evidence or technical constraints without editing product code.

Add new roles only when they remove real ambiguity. Most harness value should come from better artifacts, checks, and feedback loops rather than more role names.
