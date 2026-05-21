# Security

This repository is a template and sample project, but agents should still preserve security basics.

## Rules

- Do not commit secrets, tokens, or local credentials.
- Do not add production URLs or private infrastructure details to examples.
- Do not run destructive commands without explicit user approval.
- Keep generated reports local under `.harness-runs/`.
- Prefer source-cited docs for security-sensitive guidance.

## Review Focus

Security review should check prompts, scripts, and examples for accidental secret handling, unsafe shell behavior, and unclear destructive operations.
