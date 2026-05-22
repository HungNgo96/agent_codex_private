# Codex Configuration Decisions

## 2026-05-22 - Add Conservative Project Codex Defaults

Decision: Add project-scoped Codex defaults with workspace-write sandboxing, on-request approvals, cached web search, shallow subagent nesting, and separate plan/code/review profiles.

Reason: OpenAI Codex docs describe layered config precedence, trusted project `.codex/` loading, sandbox plus approval controls, subagent concurrency limits, and profile-scoped overrides. Encoding the defaults keeps coding sessions predictable without requiring broad permissions.

Impact: Codex users can run the same repository with a consistent configuration baseline and choose profiles for planning, implementation, or review.

## 2026-05-22 - Keep Hooks And Rules Out Until Needed

Decision: Do not add `.codex/hooks.json` or `.codex/rules/` yet.

Reason: Hooks and command rules are useful but add another trust and maintenance surface. The current workflow can be enforced through `AGENTS.md`, profiles, and reviewer coverage.

Impact: Future hooks or rules should be added only after a repeated manual policy is stable.
