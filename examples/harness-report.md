# Harness Report Examples

## Passing Report

```text
Agent harness Passed: 6 passed, 0 failed, 0 blocked, 0 warning, 0 skipped.
Markdown report: D:\HungNT97\Private\agent_codex_private\.harness-runs\20260521-120000\report.md
JSON report: D:\HungNT97\Private\agent_codex_private\.harness-runs\20260521-120000\report.json
```

Example check table:

| ID | Status | Severity | Message |
| --- | --- | --- | --- |
| required-files | Passed | Info | All required repository files are present. |
| markdown-sections | Passed | Info | Required Markdown sections are present. |
| repo-local-links | Passed | Info | Repository-local Markdown links resolve. |
| handoff-contract | Passed | Info | The handoff template includes the required evidence fields. |
| plan-contract | Passed | Info | Implementation plans include required planning fields. |
| sample-api-build | Passed | Info | The sample API builds successfully. |

## Failing Report

```text
Agent harness Failed: 4 passed, 1 failed, 0 blocked, 1 warning, 0 skipped.
Markdown report: D:\HungNT97\Private\agent_codex_private\.harness-runs\20260521-120100\report.md
JSON report: D:\HungNT97\Private\agent_codex_private\.harness-runs\20260521-120100\report.json
```

Example failed evidence:

| ID | Status | Severity | Message |
| --- | --- | --- | --- |
| markdown-sections | Failed | Error | One or more required Markdown sections are missing. |

Evidence:

- `templates/handoff-note.md: missing section 'Verification'`

## Blocked Report

```text
Agent harness Failed: 5 passed, 0 failed, 1 blocked, 0 warning, 0 skipped.
```

Blocked checks mean the repository might be valid, but the local environment cannot verify it. For example, `sample-api-build` is blocked when the .NET SDK is not installed.
