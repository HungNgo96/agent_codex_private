# Module Task History

Use this folder to keep target-specific context for Agent AI work.

Before changing a module, read that module folder first. If no module folder exists yet, create one only when the task creates durable context that future work should reuse.

## Module Folder Shape

```text
docs/modules/<module>/
  README.md
  rules.md
  decisions.md
  history.md
```

## Files

- `README.md`: what the module owns and which source folders or docs belong to it.
- `rules.md`: durable constraints that future agents must follow.
- `decisions.md`: long-lived architecture or workflow choices.
- `history.md`: short task log with date, scope, files touched, verification, and follow-up.

## Task Flow

1. Identify the target module from the user's request and affected files.
2. Read `docs/modules/<module>/README.md`, `rules.md`, `decisions.md`, and `history.md` when they exist.
3. Keep implementation scope aligned with the module ownership.
4. After a meaningful task, append one short entry to `history.md`.
5. Add decisions or rules only when they should guide future tasks, not for one-off notes.

## History Entry Template

```markdown
## YYYY-MM-DD - Short task title

- Scope:
- Files changed:
- Verification:
- Follow-up:
```
