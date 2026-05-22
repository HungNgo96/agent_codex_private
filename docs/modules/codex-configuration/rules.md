# Codex Configuration Rules

- Keep project defaults in `.codex/config.toml`; do not duplicate them in platform adapter docs.
- Treat `.codex/config.toml` as project-scoped guidance that loads only when the project `.codex/` layer is trusted.
- Use `approval_policy = "on-request"` for interactive coding.
- Use `sandbox_mode = "workspace-write"` for normal coding and `read-only` for plan/review profiles.
- Do not use `danger-full-access` as a project default.
- Keep `web_search = "cached"` unless the task explicitly needs live, current information.
- Keep `agents.max_depth = 1` unless recursive delegation is explicitly needed and reviewed.
- Keep `agents.max_threads` low enough that handoffs remain reviewable.
- Avoid repo-local skills unless they are narrow, frequently reused, and have `name` plus `description` metadata.
- Add hooks or command rules only when they enforce a stable repeated policy.
