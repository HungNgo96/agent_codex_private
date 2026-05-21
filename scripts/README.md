# Scripts

This directory contains local automation for validating the agent-team template.

## Harness

Run the local QA harness:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\Invoke-AgentHarness.ps1 -Mode Quick
```

Run full verification with sample API endpoint probes:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\Invoke-AgentHarness.ps1 -Mode Full -RunSampleApi
```

See `../docs/harness-engineering.md` for check details, report format, and extension guidance.

## Future Helpers

Potential future helpers:

- Generate task briefs from templates.
- Create timestamped handoff notes.
- Launch multiple agent sessions after the manual workflow is stable enough to automate safely.
- Aggregate worker handoffs.
- Validate additional project-specific quality gates.
