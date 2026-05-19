# Hermes-Inspired Codex Flow

This repo does not vendor or run Hermes Agent. It borrows the useful workflow ideas from Hermes Agent and maps them to Codex project agents.

Hermes Agent emphasizes a learning loop: memory, skills, self-improvement during use, past-conversation search, subagent delegation, and running across different backends. In this template, those ideas become a disciplined Codex workflow rather than a separate runtime.

## Codex Agent

Use `.codex/agents/hermes.toml` when you want a memory-aware, skill-aware coordinator.

The `hermes` agent follows this loop:

1. Recall: read `AGENTS.md`, relevant docs, previous task context, and available project skills.
2. Plan: define the smallest useful goal, ownership boundaries, verification, and risks.
3. Act: make small, reviewable changes that match existing project patterns.
4. Verify: run focused checks before claiming success.
5. Reflect: report what was learned and propose any memory or skill update as a separate verified change.

## Subagent Use

When the user explicitly requests subagents, the `hermes` agent can coordinate the existing Codex project agents:

- `leader`: analyze scope and create a plan only.
- `coder`: implement the smallest safe change.
- `reviewer`: review the final git diff.

Review must not be skipped.

## Boundaries

This flow intentionally does not add:

- Hermes runtime installation.
- Messaging gateways.
- Cron or scheduled automations.
- Provider switching.
- Terminal backend management.
- Autonomous memory writes.

Those are Hermes runtime capabilities. In Codex, this template keeps them as explicit user-approved setup work.

## Example Request

```text
Use the hermes Codex agent.
Recall repo context, plan the smallest safe change, implement it, verify it, and reflect on any reusable learning.
If subagents are useful, use leader, coder, and reviewer. Do not skip review.
```
