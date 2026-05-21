# Quality Score

Use these criteria when reviewing changes to this template.

| Area | What Good Looks Like |
| --- | --- |
| Scope | Changes are small, bounded, and aligned with existing patterns. |
| Agent usability | Prompts, templates, and docs are easy for agents to follow. |
| Verification | Every completed task reports commands run and outcomes. |
| Harness | Local checks pass or failures are explained with evidence. |
| Maintainability | New rules are documented before they are automated. |

Run the harness before reporting completion:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\Invoke-AgentHarness.ps1 -Mode Quick
```
