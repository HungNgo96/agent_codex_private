# Agent Workflow Rules

- Keep `AGENTS.md` as the primary operating contract.
- Keep platform adapters thin; Codex, Cursor, and Claude should point back to the same shared contract.
- Do not add extra agent roles unless they remove real ambiguity.
- Use `leader`, `coder`, and `reviewer` as the default explicit subagent workflow.
- Store durable module context under `docs/modules/<module>/`.
- Append task history only for meaningful workflow, docs, or behavior changes.
- Keep every role teammate-ready: explicit context read, ownership scope, avoid scope, verification expectation, handoff format, and integration notes.
- Treat custom agent TOML defaults as role intent. Live parent-session sandbox and approval overrides can still apply.
- Do not reintroduce runtime orchestration, scheduler, gateway, provider switching, or autonomous memory writes without an explicit user request.
