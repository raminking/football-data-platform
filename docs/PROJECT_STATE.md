# Football Data Platform — Project State

> Compact source of truth for continuing the project across sessions.

## Current Milestone
**Sprint 4 — Matches & Results: Match domain design completed; implementation next.**

## Completed Milestones
### Teams ✅
- Teams CRUD, domain/application tests, Carter API, PostgreSQL persistence and Testcontainers integration coverage.
- Verified milestone: **27 passed, 0 failed, 0 skipped**.
- Current Team domain intentionally remains small: `Id`, `Name`, `Country`.

### Competitions & Seasons ✅
- Competition CRUD, validation, repository, PostgreSQL persistence, Carter API, EF migration and integration coverage.
- Season domain model with Competition relationship and date-range validation.
- Season CRUD, repository, EF configuration, foreign key and unique `(CompetitionId, Name)` constraint.
- PostgreSQL/Testcontainers integration coverage.
- Verified latest full suite before Match work: **51 passed, 0 failed, 0 skipped**.

## Current Verification
Latest verified test result before Match implementation:
- **51 passed**
- **0 failed**
- **0 skipped**
- **51 total**

No Match implementation has been verified yet. The 51-test result remains the last known green baseline.

## Current Task — Matches
The Match v1 domain design is fixed and documented. Team remains unchanged. Next is implementation.

### Match v1
```text
Match
├── Id
├── SeasonId
├── HomeTeamId
├── AwayTeamId
├── ScheduledAt
├── Stage
├── Status
├── HomeScore
├── AwayScore
├── HalfTimeHomeScore
├── HalfTimeAwayScore
└── Result
```

### Relationships
```text
Competition
└── Season
    └── Match
        ├── HomeTeam → Team
        └── AwayTeam → Team
```

A Match belongs to a Season, not directly to Competition. HomeTeam and AwayTeam are two explicit relationships to the existing Team entity.

### Match Status
- Scheduled
- InProgress
- Finished
- Postponed
- Cancelled
- Abandoned

### Match Stage
Keep v1 intentionally simple:
- League
- Group Stage
- League Phase
- Playoff
- Round of 16
- Quarter Final
- Semi Final
- Final
- Friendly

### Result
- HomeWin
- Draw
- AwayWin

Result must be consistent with final scores. Prefer deriving it from scores where practical.

## Domain Boundary
The current MVP intentionally does not model:
- Extra-time score
- Penalty-shootout score
- Goals/events
- Cards
- Substitutions
- Possession
- Shots
- Corners
- Lineups
- Referee
- Venue
- Weather
- Competition format/rules
- Groups
- Season participants
- Promotion/relegation rules
- Qualification rules

These are deferred until the core model requires them.

## Team Model Decision
The current repository Team model is the source of truth for MVP:
```text
Team
├── Id
├── Name
└── Country
```

We intentionally do not add `ShortName`, `Code`, `CountryId`, or a separate Country entity at this stage. Match implementation must reference this existing Team model rather than introducing a second Team representation.

## Next Exact Steps
1. Implement Match domain types and entity using the agreed v1 model.
2. Add domain invariants, including different home/away teams and score/result consistency.
3. Implement Match application commands/queries and CRUD operations.
4. Add repository abstraction and PostgreSQL implementation.
5. Configure EF relationships for Season, HomeTeam and AwayTeam.
6. Create and verify the EF migration.
7. Add Carter API endpoints.
8. Add domain/application tests and PostgreSQL/Testcontainers integration tests.
9. Run the complete suite and record the exact result.
10. Update all strategic documentation after the Match implementation milestone.

## Success Condition
The Match module is production-oriented, persisted in PostgreSQL, exposed through the API, covered by meaningful domain and integration tests, and the complete suite remains green.

## Known Issues / Decisions To Review
- Teams update endpoint remains `POST /teams/update`; Competition and Season use conventional resource routes. Endpoint consistency can be refactored later.
- Competition-to-Team relationships are intentionally deferred except where required by Match relationships.
- Optimistic concurrency, soft delete and other advanced production concerns remain deferred until justified.
- Team remains intentionally simple until real requirements justify expansion.

## Important Decisions
- Keep tests organized by business feature first, then test type.
- Keep API request/response DTOs in `FootballDataPlatform.Contracts`.
- Use Testcontainers PostgreSQL for integration tests.
- Keep `main` as the source of truth.
- Update `docs/PROJECT_STATE.md` after every meaningful project step.
- Keep strategic documentation synchronized across `PROJECT.md`, `ROADMAP.md`, `SPRINTS.md`, and `LESSONS_LEARNED.md` when milestones change.
- Keep the Match model deliberately small; defer advanced competition and event modelling until justified.
- Keep Team aligned with the current implementation: `Id`, `Name`, `Country`.

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
