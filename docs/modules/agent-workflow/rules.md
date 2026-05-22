# Agent Workflow Rules

- Keep `AGENTS.md` as the primary operating contract.
- Keep platform adapters thin; Codex, Cursor, and Claude should point back to the same shared contract.
- Do not add extra agent roles unless they remove real ambiguity.
- Use `leader`, `coder`, and `reviewer` as the default explicit subagent workflow.
- Store durable module context under `docs/modules/<module>/`.
- Append task history only for meaningful workflow, docs, or behavior changes.
- Do not reintroduce runtime orchestration, scheduler, gateway, provider switching, or autonomous memory writes without an explicit user request.
