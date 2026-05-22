# Codex Configuration Decisions

## 2026-05-22 - Add Conservative Project Codex Defaults

Decision: Add project-scoped Codex defaults with workspace-write sandboxing, on-request approvals, cached web search, shallow subagent nesting, and separate plan/code/review profiles.

Reason: OpenAI Codex docs describe layered config precedence, trusted project `.codex/` loading, sandbox plus approval controls, subagent concurrency limits, and profile-scoped overrides. Encoding the defaults keeps coding sessions predictable without requiring broad permissions.

Impact: Codex users can run the same repository with a consistent configuration baseline and choose profiles for planning, implementation, or review.

## 2026-05-22 - Add Conservative Project Command Rules

Decision: Add `.codex/rules/default.rules` for a small set of project-local Codex command policies.

Reason: OpenAI Codex rules are designed for stable command execution policy. This template repeatedly needs safe repo search, status, diff, build, and test commands, while destructive git operations should remain blocked or explicitly prompted.

Impact: Trusted project sessions can run routine verification with fewer prompts while still preserving user work. Rules do not replace sandboxing, approvals, or reviewer coverage.

## 2026-05-22 - Add One Repo-Local Coding Skill

Decision: Add `.agents/skills/agent-ai-feature-development/SKILL.md` as the single repo-local Codex skill for repeatable feature and bug-fix work.

Reason: OpenAI Codex skills are the right format for reusable workflows because Codex can discover metadata cheaply and load full instructions only when the task matches. The AgentTeams coding flow is repeated enough to justify a skill, while additional skills would add context noise before there is a clear need.

Impact: Codex users can invoke `$agent-ai-feature-development` for coding tasks that need planning, module docs, optional leader/coder/reviewer delegation, implementation, verification, and final reporting.

## 2026-05-22 - Keep Hooks Out Until Needed

Decision: Do not add `.codex/hooks.json` yet.

Reason: Hooks add another execution surface. The current workflow can be enforced through `AGENTS.md`, profiles, command rules, and reviewer coverage.

Impact: Future hooks should be added only after a repeated manual policy is stable and cannot be handled by command rules.
