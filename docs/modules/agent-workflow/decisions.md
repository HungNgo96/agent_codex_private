# Agent Workflow Decisions

## 2026-05-22 - Replace `docs/ai` With Module Task History

Decision: Use `docs/modules/<module>/` for module ownership, rules, decisions, and task history.

Reason: `docs/ai` mixed process guidance, module docs, and architecture notes. `docs/modules` is shorter, easier to target by feature area, and still preserves task history.

Impact: Agents should read the matching module folder before making targeted changes and append short entries to `history.md` after meaningful tasks.

## 2026-05-22 - Keep Cursor And Claude As Thin Adapters

Decision: Keep `.cursor/`, `.claude/`, and `CLAUDE.md`, but treat them as adapters to the shared AgentTeams contract.

Reason: The workflow may need to run temporarily in Cursor or Claude, but maintaining separate full workflows creates drift.

Impact: Future changes should update the shared contract first, then adjust adapters only when platform-specific behavior requires it.
