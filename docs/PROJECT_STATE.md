# Football Data Platform — Project State

> Compact source of truth for continuing the project across sessions.

## Current Milestone
**Sprint 5 — Multi-Source External Data Architecture in progress.**

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
- New `IFootballDataSource` abstraction introduced.
- New `IFootballDataSourceResolver` abstraction introduced.
- `FootballDataOrgProvider` now implements `IFootballDataSource` and exposes `SourceKey = "football-data.org"`.
- Infrastructure resolver selects a source by case-insensitive source key.
- DI registers the current football-data.org source through the resolver boundary.
- Competition, Team and Season import services now resolve an `IFootballDataSource` instead of directly depending on `IFootballDataProvider`.
- `ExternalIdentity.Provider` remains the persisted source identity; external IDs never become domain primary keys.

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
 football-data.org   Future Source   Licensed FotMob*
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

`*` FotMob is a future adapter only when an authorized/licensed access path is available. The project must not depend on unauthorized scraping.

## Current External Source Contract
```text
IFootballDataSource
├── SourceKey
├── GetCompetitionsAsync()
├── GetSeasonsAsync(competitionCode)
├── GetTeamsAsync(competitionCode, seasonYear)
└── GetMatchesAsync(competitionCode, seasonYear)
```

The source adapter maps provider-specific responses into provider-neutral external records. Provider DTOs must not leak into Domain or API contracts.

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

## External Identity Boundary
```text
ExternalIdentity
├── Provider / SourceKey
├── EntityType
├── ExternalId
└── EntityId
```

The database enforces uniqueness for `(Provider, EntityType, ExternalId)`. The source key identifies the external system and remains outside domain primary keys.

## Current Verification
Latest user-reported verification before the Multi-Source refactor:
- **92 passed**
- **0 failed**
- **0 skipped**
- **92 total**

The Multi-Source changes committed in the current session have **not yet been locally verified by the user**. Do not treat 92/92 as the verified post-refactor baseline until `dotnet build` and `dotnet test` pass locally.

## Current Git State
- `main` is the only project branch and source of truth.
- No feature branch is part of the project workflow.
- Continue all work directly on `main`.

## Current Task
Finish and verify the Multi-Source refactor, including test compatibility and source resolver coverage.

## Next Exact Steps
1. Run `dotnet build` locally after the source abstraction changes.
2. Fix all remaining compile errors caused by the `IFootballDataProvider` → `IFootballDataSource` transition.
3. Update existing import tests/fakes to use `IFootballDataSourceResolver` and source keys.
4. Add resolver tests: registered source, case-insensitive lookup, unknown source and empty key.
5. Add provider adapter tests confirming `SourceKey` and Season mapping.
6. Run the complete test suite and establish the new verified baseline.
7. Review transaction/partial-failure behavior across Team, Competition and Season imports.
8. Then implement Match import using the same source-neutral boundary.
9. Add end-to-end import coverage with deterministic provider fixtures/mocks.
10. Only after synchronous import is stable, evaluate retries, rate limiting and background scheduling.

## Important Boundary Decisions
- `main` is the only source of truth.
- Do not create feature branches for this project unless explicitly agreed otherwise.
- Provider DTOs stay in Infrastructure.
- Application owns source and persistence abstractions; Infrastructure owns implementations.
- External identifiers must not become domain primary keys.
- External identity is unique by source/provider + entity type + external identifier.
- Import must be idempotent before scheduling is introduced.
- Do not add advanced match/event modelling merely to mirror a provider response.
- Season synchronization persists only the MVP fields required by the current Domain model.
- New external sources must implement `IFootballDataSource`; Import Services must not reference a concrete provider.
- FotMob integration must use an authorized access path; do not build an unauthorized scraper.

## Known Issues / Decisions To Review
- Teams update endpoint remains `POST /teams/update`; route consistency can be refactored later.
- Competition-to-Team relationships remain deferred except where required by Match relationships.
- Optimistic concurrency, soft delete and other advanced production concerns remain deferred until justified.
- Provider/source rate limiting, retry/backoff and richer error classification remain to be designed.
- Team/Competition/Season import operations currently use repository-level `SaveChangesAsync` calls and are not yet wrapped in a shared import transaction/unit-of-work.
- The current source-neutral refactor changes import method signatures to accept `sourceKey`; API/application callers and tests must be aligned and verified.

## Session Protocol
```bash
git switch main
git pull origin main
git status
git log -5 --oneline
cat docs/PROJECT_STATE.md
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
