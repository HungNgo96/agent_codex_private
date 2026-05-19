---
name: optimize-uiux
description: Use when improving Blazor UI/UX, enterprise dashboard layout, responsive screens, data grids, forms, buttons, modals, accessibility, and visual consistency.
---

# Optimize UI/UX

Use this skill when optimizing PayrollSolution screens, especially Blazor pages and shared UI components.

## Goals

- Modern, professional enterprise dashboard UI.
- Clear, low-friction UX for payroll CRUD workflows.
- Responsive desktop, tablet, and mobile layouts.
- Consistent design system, spacing, typography, colors, and component states.
- Accessible keyboard, focus, contrast, and screen-reader behavior.
- Good rendering performance for large tables and forms.

## Blazor Context

- Prefer existing project components and CSS conventions before adding new UI libraries.
- Keep Razor components focused and readable.
- Avoid broad visual rewrites unless the user asks for a redesign.
- Preserve business behavior while improving presentation and interaction flow.

## Review Checklist

- Data grids have clear search, sorting, pagination, loading, empty, and error states.
- Tables remain usable on small screens; avoid horizontal overflow unless unavoidable.
- Primary, secondary, danger, ghost, and icon buttons are visually distinct and consistent.
- Destructive actions use confirmation and clear warning copy.
- Forms have clear labels, required indicators, validation messages, and logical grouping.
- Modals are not oversized, support keyboard use, and have clear primary/secondary actions.
- Status badges use semantic colors and readable text.
- Icons have clear meaning and are not overused.
- Loading states avoid layout shift where possible.
- Accessibility basics are covered: focus states, ARIA labels where needed, contrast, and keyboard navigation.

## Output Style

- Start with the highest-impact UX issues.
- Prefer small, incremental improvements over full redesigns.
- Include concrete Blazor/Razor/CSS examples when code changes are useful.
- Explain tradeoffs when choosing between visual polish, accessibility, and implementation cost.
