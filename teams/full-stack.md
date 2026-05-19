# Full-Stack Team

Use this team for features that span multiple layers and can be split into independent slices.

## Roles

### Lead Agent

Owns requirements, interfaces, task boundaries, integration, and final verification.

### Frontend Worker

Owns UI components, client state, browser behavior, accessibility checks, and frontend tests within the assigned scope.

### Backend Worker

Owns API endpoints, service logic, validation, persistence integration, and backend tests within the assigned scope.

### Test Worker

Owns cross-layer regression tests, integration checks, and verification scripts. Frontend and backend workers own local tests for their changes; the test worker should avoid duplicating those tests unless assigned by the lead.

### Docs Worker

Owns user-facing docs, internal notes, migration instructions, or changelog entries when documentation is part of the task.

## Best Fit

- New product features.
- UI plus API changes.
- Data model changes with user-facing behavior.
- Work that can be split by directory or module.

## Avoid When

- The task is a one-file fix.
- The lead does not yet understand the required interfaces.
- Multiple workers would need to edit the same files at the same time.

## Coordination Contract

The lead must define shared interfaces before workers begin. Workers must report any interface change immediately in their handoff.
