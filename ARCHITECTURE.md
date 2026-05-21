# Architecture

This repository is a documentation-first agent-team template. It provides the operating contract, role prompts, workflow playbooks, templates, examples, and a local harness for validating the repository knowledge store.

## System Shape

```mermaid
flowchart TD
    User[User request] --> Lead[Lead agent]
    Lead --> Contract[AGENTS.md]
    Lead --> Knowledge[In-repository knowledge store]
    Lead --> Workers[Bounded workers]
    Workers --> Handoffs[Handoff notes]
    Lead --> Harness[Local QA harness]
    Harness --> Reports[Harness reports]
```

## Primary Components

- `AGENTS.md`: shared operating contract for every agent.
- `docs/`: repository knowledge store for product, design, execution plans, generated references, and quality standards.
- `prompts/`: paste-ready prompts for lead and worker sessions.
- `workflows/`: playbooks for common task types.
- `templates/`: repeatable task, handoff, review, and implementation-plan formats.
- `scripts/`: local automation, including the agent harness.
- `src/AgentTeams.SampleApi`: minimal .NET sample project used by the harness.

## Knowledge Store

The knowledge store is intentionally kept in the repository so agents can ground decisions in checked-in context. Start with `docs/PLANS.md` for active work, `docs/PRODUCT_SENSE.md` for product direction, and `docs/QUALITY_SCORE.md` for evaluation criteria.

## Verification

Run:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\Invoke-AgentHarness.ps1 -Mode Full -RunSampleApi
```
