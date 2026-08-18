# Football Data Platform — Project State

> Compact source of truth for continuing the project across sessions.

## Current Milestone
**Sprint 4 — Matches & Results: Completed and locally verified.**

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
- Verified baseline before Match work: **51 passed, 0 failed, 0 skipped**.

### Match v1 ✅
- Match domain entity and enums.
- Domain invariants for team identity and score consistency.
- Result derived from final scores.
- Application create/get/update/delete flows with MediatR.
- Match repository abstraction and PostgreSQL implementation.
- EF Core configuration with Season, HomeTeam and AwayTeam foreign keys.
- Carter API endpoints.
- Match API contracts.
- Match tests added without regression.
- PostgreSQL migration and EF model metadata.

## Current Verification
Latest local verification after Match implementation:
- **59 passed**
- **0 failed**
- **0 skipped**
- **59 total**

This is the current green baseline.

## Match v1
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

Result is derived from final scores. Finished matches require final scores.

## Domain Invariants Implemented
- SeasonId is required.
- HomeTeamId is required.
- AwayTeamId is required.
- Home and away teams must be different.
- Final scores must be supplied together.
- Half-time scores must be supplied together.
- Scores cannot be negative.
- Half-time scores cannot exceed final scores.
- Finished matches require final scores.
- Result is calculated from final scores and therefore cannot contradict them.

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

## Team Model Decision
The current repository Team model is the source of truth for MVP:
```text
Team
├── Id
├── Name
└── Country
```

We intentionally do not add `ShortName`, `Code`, `CountryId`, or a separate Country entity at this stage.

## Next Milestone — Sprint 5
**External Data Import**

Planned focus:
1. Define external-provider integration boundary.
2. Select and document the first football data provider.
3. Define provider DTOs separately from internal domain models.
4. Build mapping from provider data to Team, Competition, Season and Match.
5. Add resilient import workflow with validation, idempotency and error handling.
6. Add integration tests using provider fixtures/mocks.
7. Only then introduce background scheduling.

## Known Issues / Decisions To Review
- Teams update endpoint remains `POST /teams/update`; Competition, Season and Match use conventional resource routes. Endpoint consistency can be refactored later.
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
