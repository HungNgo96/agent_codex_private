# Tech Debt Tracker

Track known debt that is not currently assigned to an active execution plan.

| ID | Area | Issue | Impact | Suggested Next Step | Status |
| --- | --- | --- | --- | --- | --- |
| TD-001 | Harness | Git dubious-ownership warning can appear on this Windows checkout. | Harness reports a warning even when repo content is valid. | Run the documented `git safe.directory` command if Git status is needed without override. | Open |
