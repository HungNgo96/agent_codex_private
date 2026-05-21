# Database Schema

The sample API includes an employee management schema owned by `AgentTeams.SampleApi.Infrastructure`.

## employees

| Column | Type | Required | Notes |
| --- | --- | --- | --- |
| `id` | `TEXT` / provider GUID type | Yes | Primary key `pk_employees`. |
| `employee_code` | `TEXT`, max 32 | Yes | Unique business identifier. |
| `full_name` | `TEXT`, max 160 | Yes | Employee display name. |
| `email` | `TEXT`, max 256 | Yes | Basic email validation is handled in Domain. |
| `department` | `TEXT`, max 120 | Yes | Searchable field. |
| `job_title` | `TEXT`, max 120 | Yes | Employee role/title. |
| `is_active` | `INTEGER` / provider boolean type | Yes | Active flag. |
| `created_at` | `TEXT` / provider timestamp type | Yes | UTC timestamp. |
| `updated_at` | `TEXT` / provider timestamp type | Yes | UTC timestamp. |

## Indexes

- `ix_employees_employee_code`: unique index on `employee_code`.

## Migrations

- Initial migration: `src/AgentTeams.SampleApi.Infrastructure/Migrations/20260521000000_InitialEmployeePersistence.cs`.
- SQLite is the default provider.
- PostgreSQL can be selected through `Database:Provider = PostgreSql` and the `EmployeeDatabase` connection string.
- Startup auto-migration is enabled for SQLite only in this sample. Generate and review PostgreSQL-specific migrations before applying them to a PostgreSQL database.
