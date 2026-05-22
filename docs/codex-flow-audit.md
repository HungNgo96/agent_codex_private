# Codex Flow Audit

This audit reviews the agent-team flow outside `src/` and aligns the template with the current Codex documentation.

## Source References

- Codex `AGENTS.md` discovery: https://developers.openai.com/codex/guides/agents-md
- Codex subagents: https://developers.openai.com/codex/subagents
- Codex skills: https://developers.openai.com/codex/skills
- Codex workflows: https://developers.openai.com/codex/workflows
- Codex permissions: https://developers.openai.com/codex/permissions
- Codex sandboxing: https://developers.openai.com/codex/concepts/sandboxing
- Codex config basics: https://developers.openai.com/codex/config-basic
- Codex config reference: https://developers.openai.com/codex/config-reference
- Codex hooks: https://developers.openai.com/codex/hooks

## Current Fit

- `AGENTS.md` is the right always-on project contract. Codex reads `AGENTS.md` files before work and layers project guidance from the project root down to the current working directory.
- `.codex/agents/*.toml` matches the Codex custom agent shape. Each project agent defines `name`, `description`, and `developer_instructions`.
- `.codex/config.toml` keeps subagent fan-out bounded with `max_threads = 4` and `max_depth = 1`, and provides plan/code/review profiles.
- `prompts/`, `workflows/`, and `templates/` give the lead enough structure to create bounded task briefs, collect handoffs, and run verification.
- `docs/modules/` preserves module-level task history without the larger `docs/ai` knowledge-store layer.

## Required Adjustments

- Treat `AGENTS.md` as the only repo file Codex reliably loads automatically. Other docs, prompts, workflows, templates, and module history files should be explicitly mentioned, attached, or read by the lead before use.
- Keep Codex subagents opt-in. Codex spawns subagents only when the user explicitly requests that workflow, and the main agent still owns coordination, integration, final review, and verification.
- Preserve parent runtime controls. Subagents inherit the active sandbox and approval behavior from the parent session, so prompts should not imply that agent files can always override live permissions.
- Keep custom agents narrow. The default project agents are `leader`, `coder`, and `reviewer`.
- Keep skills outside the template unless they are intentionally installed for the project. If repo-local skills are reintroduced later, every skill should include `name` and `description` metadata.
- Keep hooks and command rules out of the template until a stable repeated policy needs deterministic enforcement.

## Standard Codex Implementation Flow

1. Start from `AGENTS.md`, then explicitly read the relevant files from `README.md`, `ARCHITECTURE.md`, `docs/agent-ai-task-flow.md`, `docs/modules/`, `workflows/`, and `templates/`.
2. Define success criteria, editable scope, avoid scope, shared contracts, and verification before assigning work.
3. Use a single main agent for small or tightly coupled tasks.
4. Use subagents only when the user asks for them and the work splits into independent scopes with low conflict risk.
5. Assign each worker a bounded ownership scope and tell them they are not alone in the codebase.
6. Review all handoffs before integration.
7. Run verification that matches the changed surface. For docs-only changes, prefer static checks and link checks.
8. Report changed files, commands run, evidence, risks, and skipped verification reasons.

## Configuration Profiles

- `plan`: read-only planning and research with higher reasoning.
- `code`: normal implementation with workspace writes and on-request approvals.
- `review`: read-only final diff review with higher reasoning.

## Out Of Scope

- No changes to `src/`.
- No new multi-agent runtime.
- No scheduler, messaging gateway, provider switching, or background service.
- No autonomous memory writes.
