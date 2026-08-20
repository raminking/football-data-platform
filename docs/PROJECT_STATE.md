# Football Data Platform — Project State

> **Source of truth for continuing the project across sessions.** Keep this file synchronized after every meaningful milestone so project context is never lost.

## Current Milestone
**Sprint 5 — Multi-Source External Data Architecture / Import Foundation**

## Verified Status
- **Tests: 97 passed, 0 failed, 0 skipped, 97 total.**
- The 97-test result was locally verified by the user after the latest fixes.
- `main` is the only project branch and the only branch to use.
- The project currently has the Multi-Source source abstraction in place.
- The database schema/persistence foundation exists, but **real football data has not yet been confirmed as imported into the user's database**.

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

### Match v1 ✅
- Match domain entity and enums.
- Domain invariants for team identity and score consistency.
- Result derived from final scores.
- Application create/get/update/delete flows with MediatR.
- Match repository abstraction and PostgreSQL implementation.
- EF Core configuration with Season, HomeTeam and AwayTeam foreign keys.
- Carter API endpoints and contracts.
- PostgreSQL migration and EF model metadata.

### External Identity & Import Foundation ✅
- Persistent `ExternalIdentity` support for provider/entity/external-id mapping.
- Unique `(Provider, EntityType, ExternalId)` database constraint.
- External identity repository abstraction and PostgreSQL implementation.
- Competition, Team and Season import services implemented.
- Create/update/idempotency behavior covered by unit and PostgreSQL integration tests.
- Season import resolves Competition through persisted external identity with provider-code fallback.

## Multi-Source External Data Architecture 🚧
- Provider-specific DTOs remain inside Infrastructure.
- Provider-neutral `ExternalCompetition`, `ExternalSeason`, `ExternalTeam` and `ExternalMatch` records remain in Application abstractions.
- `IFootballDataSource` abstraction introduced.
- `IFootballDataSourceResolver` abstraction introduced.
- `FootballDataOrgProvider` implements `IFootballDataSource` and exposes `SourceKey = "football-data.org"`.
- Infrastructure resolver selects a source by case-insensitive source key.
- DI registers the current football-data.org source through the resolver boundary.
- Competition, Team and Season import services resolve an `IFootballDataSource` instead of directly depending on a concrete provider.
- External identities remain source-scoped; external IDs never become domain primary keys.

### Target source architecture
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

**FotMob decision:** FotMob is **not** a production dependency. It may only be considered later if an authorized/licensed access path is available. The project must not depend on unauthorized scraping or reverse-engineered private endpoints.

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

## Import Workflows
### Team
```text
sourceKey
   ↓
IFootballDataSourceResolver
   ↓
IFootballDataSource.GetTeamsAsync
   ↓
TeamImportService
   ↓
ExternalIdentity(source, Team, externalId)
   ├── found → update Team
   └── missing → create Team + identity
```

### Competition
```text
sourceKey
   ↓
IFootballDataSourceResolver
   ↓
source.GetCompetitionsAsync
   ↓
CompetitionImportService
   ↓
ExternalIdentity(source, Competition, externalId)
   ├── found → update Competition
   └── missing → create Competition + identity
```

### Season
```text
sourceKey + competitionCode
   ↓
IFootballDataSourceResolver
   ↓
source.GetSeasonsAsync
   ↓
Resolve Competition ExternalIdentity
   ↓
Load internal Competition
   ↓
ExternalIdentity(source, Season, externalId)
   ├── found → update Season
   └── missing → create Season + identity
```

### Match — next import milestone
```text
sourceKey + competitionCode + seasonYear
   ↓
IFootballDataSourceResolver
   ↓
source.GetMatchesAsync
   ↓
Resolve Competition / Season / HomeTeam / AwayTeam identities
   ↓
MatchImportService
   ↓
Match + ExternalIdentity
```

## External Identity Boundary
```text
ExternalIdentity
├── Provider / SourceKey
├── EntityType
├── ExternalId
└── EntityId
```

The database enforces uniqueness for `(Provider, EntityType, ExternalId)`. The source key identifies the external system and remains outside domain primary keys.

## Database / Real Data Status
- PostgreSQL persistence and migrations are implemented.
- Integration tests use deterministic infrastructure/test data.
- **No claim is currently made that the user's local database contains live football-data.org records.**
- The next data milestone is an explicitly verified end-to-end import from `football-data.org` into PostgreSQL, followed by querying/counting the persisted records.

## Current Git State
- `main` is the only source of truth.
- Do not create feature branches or other branches for this project unless explicitly agreed otherwise.
- All project work continues directly on `main`.

## Current Task
Move from the verified Multi-Source foundation to a real, observable import pipeline:
1. Complete source resolver/priority/fallback behavior.
2. Verify Team/Competition/Season imports against deterministic sources.
3. Implement Match import through the source-neutral boundary.
4. Add an end-to-end import path using `football-data.org` and verify persisted PostgreSQL records.
5. Define transaction/partial-failure behavior.

## Next Exact Steps
1. Inspect current import APIs and persistence flow on `main`.
2. Add/verify source priority and safe fallback semantics without coupling Application to providers.
3. Add resolver tests for registered source, case-insensitive lookup, unknown source and empty key.
4. Finish Match import and its identity resolution.
5. Add deterministic end-to-end import coverage.
6. Run `dotnet build` and the complete `dotnet test` suite.
7. Execute a real football-data.org import against the configured local PostgreSQL database and record the verified result.
8. Review transaction/partial-failure behavior across all import services.
9. Only after synchronous import is stable, evaluate retries, rate limiting and background scheduling.

## Important Boundary Decisions
- `main` is the only source of truth.
- Do not create feature branches for this project unless explicitly agreed otherwise.
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
- Team/Competition/Season import operations currently use repository-level `SaveChangesAsync` calls and are not yet wrapped in a shared import transaction/unit-of-work.
- Real-data database population has not yet been verified.

## Verified Baseline
```text
97 passed
0 failed
0 skipped
97 total
```

## Session Protocol
```bash
git switch main
git pull origin main
git status
git log -5 --oneline
cat docs/PROJECT_STATE.md
dotnet clean
dotnet build
dotnet test
```

After each meaningful milestone, update:
- Current Milestone
- Completed / Previous Milestone
- Current Task
- Next Exact Steps
- Known Issues / Decisions
- Test verification result
- Database/live-data verification status
