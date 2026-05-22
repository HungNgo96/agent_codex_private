# Codex Configuration Module

This module owns Codex configuration guidance for the AgentTeams template.

## Owned Areas

- `.codex/config.toml`
- `.codex/agents/`
- `.agents/skills/`
- `AGENTS.md`
- `docs/codex-flow-audit.md`
- `docs/agent-ai-task-flow.md`
- `docs/modules/codex-configuration/`

## Source References

- Codex config basics: https://developers.openai.com/codex/config-basic
- Codex config reference: https://developers.openai.com/codex/config-reference
- Codex permissions: https://developers.openai.com/codex/permissions
- Codex sandboxing: https://developers.openai.com/codex/concepts/sandboxing
- Codex subagents: https://developers.openai.com/codex/subagents
- Codex skills: https://developers.openai.com/codex/skills
- Codex workflows: https://developers.openai.com/codex/workflows
- Codex hooks: https://developers.openai.com/codex/hooks
- Codex rules: https://developers.openai.com/codex/rules

## Configuration Shape

Project defaults live in `.codex/config.toml`. Profile definitions are intentionally not kept in the project template because Codex profile configuration is user-level in the current setup. Keep `plan`, `code`, and `review` profiles in the user's Codex config when those shortcuts are needed:

- `--profile plan`: read-only planning and research.
- `--profile code`: normal implementation with workspace writes.
- `--profile review`: read-only final review.

The default project mode is intentionally conservative: `approval_policy = "on-request"`, `sandbox_mode = "workspace-write"`, and `web_search = "cached"`. The project file owns shared defaults, command rules, and agent fan-out limits; user-level config owns personal profile shortcuts.

## Command Rules

Project command rules live in `.codex/rules/default.rules`. Codex loads them only when the project `.codex/` layer is trusted.

Rules are not a replacement for sandboxing or human review. Use them for stable, repeated command policy:

- `allow` for routine read-only or verification commands such as `rg`, `git status`, `git diff`, `dotnet build`, and `dotnet test`.
- `prompt` for commands that can publish or remove work, such as `git push`, `git clean`, and recursive deletion.
- `forbidden` for destructive commands that violate the operating contract, such as `git reset --hard`.

After changing a rule, test it with `codex execpolicy check --pretty --rules .codex/rules/default.rules -- <command>`, then restart Codex so the active session reloads rules.

## Skills

Repo-local skills live in `.agents/skills/`. Codex scans this folder from the current working directory up to the repository root and loads only each skill's metadata until a matching task needs the full `SKILL.md`.

Use skills for reusable workflows, not general rules:

- `AGENTS.md`: always-on operating contract.
- `.codex/agents/`: role definitions for explicit subagent work.
- `.agents/skills/`: reusable task workflow instructions that Codex can invoke explicitly or implicitly.
- `docs/modules/<module>/`: durable module context and task history.

The template includes `$agent-ai-feature-development` as the single repo-local coding workflow skill. Add another skill only when the workflow is repeated, has clear trigger wording, and cannot stay simpler as a module rule or workflow doc.

## Practical Coding Flow

1. Start at the repo root so Codex can find the project root and `AGENTS.md`.
2. Use a user-level `--profile plan` or the `leader` agent for scope analysis.
3. Use a user-level `--profile code` or the `coder` agent for implementation.
4. Use a user-level `--profile review` or the `reviewer` agent for final diff review.
5. Keep subagent fan-out shallow: direct children only, no recursive delegation.
6. Use `$agent-ai-feature-development` for repeatable feature or bug-fix coding work.
7. Prefer repo docs and module history over broad skill/plugin installs.
8. Keep command rules narrow, tested, and aligned with the agent operating contract.
9. Add hooks only after a repeated need is proven.
