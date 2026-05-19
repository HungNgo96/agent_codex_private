# Debugging Team

Use this team when a failure has multiple plausible causes that can be investigated independently.

## Roles

### Lead Agent

Owns reproduction, observed facts, hypothesis tracking, fix selection, and final verification.

### Reproduction Worker

Creates or confirms the smallest reliable reproduction. Reports exact steps, command output, and observed failure.

### Code Path Worker

Traces the relevant implementation path and identifies where behavior diverges from expectations.

### Environment Worker

Checks configuration, dependency versions, runtime settings, generated artifacts, and external service assumptions.

### Verification Worker

Runs focused checks after a candidate fix and confirms the original failure no longer reproduces.

## Best Fit

- Flaky tests.
- Production-only failures.
- Configuration-sensitive bugs.
- Performance regressions with multiple possible bottlenecks.

## Debugging Rules

Do not patch or launch broad parallel investigation before reproducing the issue or establishing a clear fact baseline. Parallelize only after the lead has enough facts to assign independent hypotheses, and keep a visible list so workers do not duplicate effort.
