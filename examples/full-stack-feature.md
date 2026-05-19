# Example: Full-Stack Feature Team

## Scenario

Add a comments feature to an existing web application.

## Lead Brief

Goal: Users can view and submit comments on an article page.

Non-goals: Moderation, notifications, rich text, and admin tools are out of scope.

Success criteria:

- Article pages show existing comments.
- Signed-in users can submit a comment.
- Empty comments are rejected.
- Tests cover API validation and the primary UI flow.

## Team Assignment

| Agent | Edit scope | Avoid scope | Shared interfaces/dependencies | Output |
| --- | --- | --- | --- | --- |
| Lead | Comment contracts, integration notes, final verification | Worker-owned implementation files unless blocking | Comment read model, create request/response, auth assumptions | Working feature and final report |
| Frontend Worker | Article comments UI and assigned frontend tests | API handlers, persistence, unrelated layout refactors | Comment read model and create-comment response | UI implementation and scoped frontend checks |
| Backend Worker | Comments API, validation, persistence, and assigned backend tests | Article UI files and unrelated data models | Create-comment request/response, signed-in user identity | API implementation and scoped backend checks |
| Test Worker | Cross-layer regression and integration tests | Duplicating frontend/backend local tests unless lead assigns it | Public UI flow, API contract, auth behavior | Integration coverage and test results |

## Worker Dispatch Example

```text
You are the Frontend Worker. You are not alone in the codebase; do not revert others' changes.

Goal: Add the comments UI to the article page.
Ownership: `src/article/` UI components and related frontend tests.
Avoid: API handlers, database files, unrelated layout refactors.
Shared interfaces/dependencies: Comment read model, create-comment response, and server-derived author identity.
Verification: Run relevant scoped frontend checks. If no check can be run, state the skipped-verification reason explicitly.
Handoff: Report scope completed, files changed or evidence gathered, commands run, results, risks/assumptions, integration notes, and suggested next step.
```

## Integration Notes

The lead should define separate read and create contracts before dispatching workers. `authorName` and any author identity are derived server-side from the signed-in user; the client must not send or be trusted for those fields.

Comment read model:

```json
{
  "id": "string",
  "articleId": "string",
  "authorName": "string",
  "body": "string",
  "createdAt": "ISO-8601 string"
}
```

Create-comment request:

```json
{
  "body": "string"
}
```

Create-comment response:

```json
{
  "comment": {
    "id": "string",
    "articleId": "string",
    "authorName": "string",
    "body": "string",
    "createdAt": "ISO-8601 string"
  }
}
```

Frontend and backend workers own local tests for their changes. The Test Worker owns cross-layer regression and integration tests, and should avoid duplicating those local tests unless the lead assigns that coverage.

The lead integrates worker handoffs, resolves contract mismatches, and runs the final project verification commands.
