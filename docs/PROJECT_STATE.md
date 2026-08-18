# Football Data Platform — Project State

> Compact source of truth for continuing the project across sessions.

## Current Milestone
**Sprint 4 — Matches & Results: starting Match domain design.**

## Completed Milestones
### Teams ✅
- Teams CRUD, domain/application tests, Carter API, PostgreSQL persistence and Testcontainers integration coverage.
- Verified: **27 passed, 0 failed, 0 skipped**.

### Competitions ✅
- Competition CRUD, validation, repository, PostgreSQL persistence, Carter API, EF migration and integration coverage.
- Verified as part of the current full suite.
- EF migration designer issue was fixed and the migration is now correctly discovered/applied.

### Seasons ✅
- Season domain model with Competition relationship and date-range validation.
- Create/Get/Update/Delete application operations.
- Repository abstraction and PostgreSQL implementation.
- EF configuration, foreign key and unique `(CompetitionId, Name)` constraint.
- Carter CRUD API endpoints.
- PostgreSQL/Testcontainers integration coverage.
- EF Core migration `20260818124212_AddSeasons` and model snapshot.
- Verified: **51 passed, 0 failed, 0 skipped**.

## Current Verification
Latest local full-suite result:
- **51 passed**
- **0 failed**
- **0 skipped**
- **51 total**

Git working tree is clean and `main` is synchronized with `origin/main`.

## Current Task — Matches
Design and implement the Match slice as the next core football-data capability.

Initial Match scope:
- Match domain entity.
- Competition Season relationship.
- Home Team / Away Team relationships.
- Kickoff date/time.
- Match status.
- Score/result model.
- Domain validation, including preventing the same team from being both home and away.
- CRUD/application operations.
- PostgreSQL persistence and migration.
- Carter API.
- Unit/domain tests and PostgreSQL integration tests.

## Next Exact Steps
1. Inspect existing Team, Competition and Season domain conventions before implementing Match.
2. Define Match status and score/result model without over-engineering.
3. Implement Match domain and application slices.
4. Add EF configuration and migration.
5. Add API endpoints.
6. Add integration/domain tests.
7. Run the complete suite and record the exact result.
8. Update all strategic docs after the Match milestone.

## Success Condition
The Match module is production-oriented, persisted in PostgreSQL, exposed through the API, covered by meaningful domain and integration tests, and the complete suite remains green.

## Known Issues / Decisions To Review
- Teams update endpoint remains `POST /teams/update`; Competition and Season use conventional resource routes. Endpoint consistency can be refactored later.
- Competition-to-Team relationships are intentionally deferred except where required by Match relationships.
- Optimistic concurrency, soft delete and other advanced production concerns remain deferred until justified.

## Important Decisions
- Keep tests organized by business feature first, then test type.
- Keep API request/response DTOs in `FootballDataPlatform.Contracts`.
- Use Testcontainers PostgreSQL for integration tests.
- Keep `main` as the source of truth.
- Update `docs/PROJECT_STATE.md` after every meaningful project step.
- Keep strategic documentation synchronized across `PROJECT.md`, `ROADMAP.md`, `SPRINTS.md`, and `LESSONS_LEARNED.md` when milestones change.

## Session Protocol
```bash
git status
git log -5 --oneline
cat docs/PROJECT_STATE.md
dotnet test
```

After each meaningful milestone, update:
- Current Milestone
- Completed / Previous Milestone
- Current Task
- Next Exact Steps
- Known Issues / Decisions
- Test verification result
