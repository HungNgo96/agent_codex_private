# Architecture Decisions

## 2026-05-17 - Keep module docs under docs/ai

### Decision

AI-facing module documentation lives under `docs/ai`, grouped by business module.

### Reason

AI agents need stable business context before changing payroll, employee, email, tax, contract or architecture behavior.

### Implementation Notes

- Module folders use `README.md`, `rules.md` and `decisions.md`.
- Short implementation notes stay in chat unless the task warrants saved docs.

---

## 2026-05-17 - EF configuration stays colocated by model concern

### Decision

Entity Framework schema configuration stays under `src/Data/Configurations`.

### Reason

This keeps table/index/length rules discoverable without bloating `AppDbContext`.

### Implementation Notes

- `AppDbContext` applies configurations from assembly.
- New entity schema rules should be added as configuration classes.

---

## 2026-05-17 - Services are the business boundary for UI

### Decision

Razor components should call services/repositories instead of embedding cross-module business rules directly in UI.

### Reason

Payroll, email, tax and employee logic is shared across import, export, form and batch workflows.

### Implementation Notes

- Keep UI focused on input, feedback and orchestration.
- Put reusable validation/calculation/data access behavior in services.
