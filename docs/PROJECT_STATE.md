# Football Data Platform — Project State

> **Source of truth for continuing the project across sessions.** Keep this file synchronized after every meaningful milestone.

## Current Milestone
**Sprint 5 — Multi-Source External Data & Import Pipeline**

## Verified Status
- **Tests: 102 passed, 0 failed, 0 skipped, 102 total.**
- The complete suite was locally verified by the user after the latest fixes.
- `main` is the only project branch and the only branch to use.
- Competition, Season, Team and Match CRUD are implemented.
- Provider-neutral external data abstractions and persistent external identities are implemented.
- Competition, Season, Team and Match imports are implemented through the source-neutral boundary.
- `FootballDataImportOrchestrator` coordinates the end-to-end import flow.
- `GET /imports/status` exposes persisted database counts.
- A real local PostgreSQL import has been verified for Premier League 2025/26: first run `541 created / 0 updated / 0 skipped`; repeated run `0 created / 541 updated / 0 skipped`.

## Completed Milestones
### Teams ✅
- Teams CRUD, domain/application tests, Carter API, PostgreSQL persistence and Testcontainers integration coverage.
- Team metadata support introduced for current requirements.
- Application Team import implemented and idempotent.
- PostgreSQL integration coverage verifies Team and ExternalIdentity persistence across repeated imports.

### Competitions & Seasons ✅
- Competition CRUD, validation, repository, PostgreSQL persistence, Carter API, EF migration and integration coverage.
- Season domain model with Competition relationship and date-range validation.
- Season CRUD, repository, EF configuration, foreign key and unique `(CompetitionId, Name)` constraint.
- Competition and Season API tests use public IDs at the API boundary.

### Match v1 ✅
- Match domain entity and enums.
- Domain invariants for team identity and score consistency.
- Result derived from final scores.
- Application create/get/update/delete flows with MediatR.
- Match repository abstraction and PostgreSQL implementation.
- EF Core configuration with Season, HomeTeam and AwayTeam foreign keys.
- Carter API endpoints and contracts.
- PostgreSQL migration and EF model metadata.
- Match import and ExternalIdentity resolution are implemented and covered by the full suite.

### Internal/Public ID Separation ✅
- Domain entities use internal `long` database identifiers.
- Public API identifiers use `Guid` values (`PublicId`).
- External identifiers never become domain primary keys.
- EF migrations and model snapshot reflect the separation.
- API integration tests verify public IDs for Competition and Season.

### External Identity & Import Foundation ✅
- Persistent `ExternalIdentity` support for source/provider/entity/external-id mapping.
- Unique `(Provider, EntityType, ExternalId)` database constraint.
- External identity repository abstraction and PostgreSQL implementation.
- Competition, Team, Season and Match import services implemented.
- Create/update/idempotency behavior covered by unit and PostgreSQL integration tests.
- Season import resolves Competition through persisted external identity with provider-code fallback.

## Multi-Source External Data Architecture ✅
- Provider-specific DTOs remain inside Infrastructure.
- Provider-neutral `ExternalCompetition`, `ExternalSeason`, `ExternalTeam` and `ExternalMatch` records remain in Application abstractions.
- `IFootballDataSource` abstraction introduced.
- `IFootballDataSourceResolver` abstraction introduced.
- `FootballDataOrgProvider` implements `IFootballDataSource` and exposes `SourceKey = "football-data.org"`.
- Infrastructure resolver selects a source by case-insensitive source key.
- DI registers the current football-data.org source through the resolver boundary.
- Import services resolve an `IFootballDataSource` instead of directly depending on a concrete provider.
- External identities remain source-scoped.

### Source architecture
```text
                    Application
                         │
             IFootballDataSourceResolver
                         │
                IFootballDataSource
                         │
          ┌──────────────┼──────────────┐
          ▼              ▼              ▼
 football-data.org   Future Official   Other Authorized
      provider           Source             Source
          │
          ▼
 ExternalCompetition / ExternalSeason / ExternalTeam / ExternalMatch
          │
          ▼
       Import Services
          │
          ▼
    ExternalIdentity
          │
          ▼
       Domain + DB
```

**FotMob decision:** FotMob is not a production dependency. It may only be considered later if an authorized/licensed access path is available. The project must not depend on unauthorized scraping or reverse-engineered private endpoints.

## Current External Source Contract
```text
IFootballDataSource
├── SourceKey
├── GetCompetitionsAsync()
├── GetSeasonsAsync(competitionCode)
├── GetTeamsAsync(competitionCode, seasonYear)
└── GetMatchesAsync(competitionCode, seasonYear)
```

Provider-specific responses are mapped to provider-neutral external records. Provider DTOs must not leak into Domain or API contracts.

## Import Pipeline
```text
sourceKey + competitionCode + seasonYear
                ↓
IFootballDataSourceResolver
                ↓
IFootballDataSource
                ↓
FootballDataImportOrchestrator
                ↓
Competition → Season → Teams → Matches
                ↓
ExternalIdentity resolution
                ↓
Domain entities + PostgreSQL
```

Imports are idempotent. Existing external identities are resolved and records are updated rather than duplicated.

## Import API
```text
POST /imports/{sourceKey}/{competitionCode}/{seasonYear}
GET  /imports/status
```

Example:
```text
POST /imports/football-data.org/PL/2025
```

The import response aggregates `Created`, `Updated`, `Skipped`, `Processed` and `Errors` totals.

The status endpoint reports persisted counts for competitions, seasons, teams, matches and external identities.

## Verified Real-Data Import
The configured local PostgreSQL database has been exercised through the import API with Premier League 2025/26:

```text
First run:    541 created / 0 updated / 0 skipped
Second run:     0 created / 541 updated / 0 skipped
```

This verifies the current match import path is idempotent for that dataset. Actual database row counts remain runtime state and can change independently of Git.

## Database
- PostgreSQL + Entity Framework Core.
- Migrations are stored under `src/FootballDataPlatform.Infrastructure/Migrations/`.
- The API applies pending EF Core migrations at startup when the configured database is reachable.
- Manual migration command:
```bash
dotnet ef database update
```

## Current Git State
- `main` is the only source of truth.
- Do not create feature branches for this project unless explicitly agreed otherwise.
- All project work continues directly on `main`.

## Current Task
Move from the verified import foundation toward production-ready ingestion:
1. Review source priority/fallback behavior.
2. Define transaction and partial-failure behavior across imports.
3. Improve provider error classification, retry/backoff and rate limiting.
4. Add operational observability around import runs.
5. Only after synchronous import is stable, evaluate background scheduling.

## Next Exact Steps
1. Inspect current import transaction boundaries and failure semantics.
2. Add/verify resolver priority and safe fallback tests.
3. Add import transaction/unit-of-work behavior where justified.
4. Add structured import error classification and provider diagnostics.
5. Evaluate rate limiting and retry/backoff against provider constraints.
6. Add operational logging/metrics and health checks.
7. Run `dotnet build` and the complete `dotnet test` suite after each milestone.
8. Re-verify real-data import and `/imports/status` when import behavior changes.

## Important Boundary Decisions
- `main` is the only source of truth.
- Provider DTOs stay in Infrastructure.
- Application owns source and persistence abstractions; Infrastructure owns implementations.
- External identifiers must not become domain primary keys.
- External identity is unique by source/provider + entity type + external identifier.
- Import must be idempotent before scheduling is introduced.
- Do not add advanced match/event modelling merely to mirror a provider response.
- Season synchronization persists only MVP fields required by the current Domain model.
- New external sources must implement `IFootballDataSource`; Import Services must not reference a concrete provider.
- FotMob is not a production source unless an authorized/licensed access path is established.

## Known Issues / Decisions To Review
- Teams update endpoint remains `POST /teams/update`; route consistency can be refactored later.
- Competition-to-Team relationships remain deferred except where required by Match relationships.
- Optimistic concurrency, soft delete and other advanced production concerns remain deferred until justified.
- Provider/source rate limiting, retry/backoff and richer error classification remain to be designed.
- Import transaction/partial-failure behavior needs an explicit production decision.

## Verified Baseline
```text
102 passed
0 failed
0 skipped
102 total
```

## Session Protocol
```bash
git switch main
git pull --ff-only origin main
git status
git log -5 --oneline
cat docs/PROJECT_STATE.md
dotnet clean
dotnet build
dotnet test
```

After each meaningful milestone, update Current Milestone, completed work, current task, next steps, known issues and test verification.