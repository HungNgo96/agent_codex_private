# Employee Basic Info Decisions

## 2026-05-22 - Use Existing Employee Aggregate

Decision: Save basic employee information through the existing employee aggregate and repository.

Reason: The sample API already has validation, duplicate-code checks, persistence, and read-back behavior for employees. Reusing it keeps the feature small and avoids schema churn.

Impact: The basic-info endpoint defaults `Department` to `General` and `JobTitle` to `Employee`.
