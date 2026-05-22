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
- Keep project-local command rules in `.codex/rules/default.rules`.
- Use `allow` rules only for routine read-only or verification commands.
- Use `prompt` rules for commands that publish, delete, clean, install globally, or mutate state outside the workspace.
- Use `forbidden` rules for commands that conflict with the operating contract, especially broad destructive git operations.
- Include `match` and `not_match` examples for every non-trivial `prefix_rule`.
- Test rules with `codex execpolicy check --pretty --rules .codex/rules/default.rules -- <command>` before relying on them.
- Do not use command rules to bypass review. The reviewer role still checks the final diff and verification evidence.
- Add hooks only when they enforce a stable repeated policy that command rules cannot cover.
