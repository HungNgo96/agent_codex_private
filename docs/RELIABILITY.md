# Reliability

Reliability for this template means repeatable agent execution and trustworthy completion evidence.

## Practices

- Keep `AGENTS.md` short and authoritative.
- Require bounded ownership for worker tasks.
- Require handoff evidence before integration.
- Run the harness after documentation or workflow changes.
- Treat environment blockers separately from repository failures.

## Known Environment Risk

On this checkout, Git may report dubious ownership unless `safe.directory` is configured. The harness records that as environment evidence and does not mutate global Git settings.
