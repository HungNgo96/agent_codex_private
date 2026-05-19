# Code Review Workflow

Use this workflow when reviewing proposed changes.

## Steps

1. Lead gathers changed files and relevant context.
2. Reviewer inspects correctness, regressions, security, maintainability, and missing tests.
3. Findings are reported first, ordered by severity.
4. Each finding includes a file and line reference when possible.
5. Reviewer separates confirmed issues from questions and assumptions.
6. Lead summarizes residual risk and verification gaps.

The reviewer may be the lead or a delegated review worker. The lead owns the final review artifact and residual-risk summary.

## Finding Format

Use this format:

```text
Severity: High | Medium | Low
File: path/to/file.ext:line
Issue: What is wrong.
Impact: What can break.
Recommendation: Concrete fix or next check.
```

## Review Standard

Do not pad the review with style preferences. Report issues that matter for correctness, safety, user behavior, or maintainability.
