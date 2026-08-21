# Football Data Platform — Project State

> **Source of truth for continuing the project across sessions.** Keep this file synchronized after every meaningful milestone.

## Current Milestone
**Sprint 5.5 — Extensible Football Analytics Domain Blueprint**

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

## Extensible Football Analytics Domain Direction

The domain blueprint is now explicitly documented in `docs/DOMAIN_MODEL.md` and the roadmap in `docs/ROADMAP.md`.

### Principles now agreed
- The project is a football analytics data platform, not only a CRUD API.
- Keep the current core small while preserving extension points.
- Player is an independent entity and is never deleted because a player leaves football or retires.
- Player retirement is represented by a nullable `RetiredAt` lifecycle value.
- Player positions are a capability/profile relationship; actual match position is match context.
- Player-Team history is represented separately and remains historical.
- Match lineup stores only the selected starters and substitutes, not the entire roster.
- Predicted lineup and actual lineup are separate facts.
- Player availability for a match is a structured concept for injury, suspension, doubtful status, illness, coach decision and similar known facts.
- Coach is independent and team coaching history is time-bounded.
- Formation is distinct from both Player position and Team identity; team tactical profile is historical, while match formation can change during a match.
- Match officials are independent entities with match-specific roles.
- Match events are first-class future domain data and must remain separate from derived statistics.
- Historical data is preserved; destructive deletion is not the default for football entities.
- Provider DTOs remain outside the Domain.
- Missing source data is not replaced with synthetic records.
- No speculative big-bang implementation is required; these are extension targets.

### Target domain shape
```text
Player
 ├── PlayerPosition → Position
 └── PlayerTeamAssignment → Team

Team
 └── TeamCoachAssignment → Coach

Match
 └── MatchTeam
      ├── MatchLineup
      ├── PredictedMatchLineup
      ├── PlayerAvailability
      └── MatchTeamFormation

Match
 ├── MatchOfficial
 └── MatchEvent

Team
 └── TeamTacticalProfile
```

These concepts are intentionally documented before implementation so future capabilities can be added without forcing a redesign of the current Competition → Season → Match foundation.

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
Complete production-ready ingestion engineering before adding the first Player domain implementation:
1. Review current import transaction boundaries and failure semantics.
2. Define source priority/fallback behavior.
3. Add structured provider error classification and diagnostics.
4. Evaluate retry/backoff and rate limiting against provider constraints.
5. Add operational logging/metrics and health checks.
6. Only after synchronous import is stable, evaluate background scheduling.
7. Then implement Player + Position + historical PlayerTeamAssignment as the first real football-domain extension.

## Next Exact Steps
1. Inspect current import transaction boundaries and failure semantics.
2. Add/verify resolver priority and safe fallback tests.
3. Add import transaction/unit-of-work behavior where justified.
4. Add structured import error classification and provider diagnostics.
5. Evaluate rate limiting and retry/backoff against provider constraints.
6. Add operational logging/metrics and health checks.
7. Run `dotnet build` and the complete `dotnet test` suite after each milestone.
8. Re-verify real-data import and `/imports/status` when import behavior changes.
9. Start Player + Position + PlayerTeamAssignment only after the ingestion foundation is stable.

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
- Future football-domain extensions must be additive and source-neutral where possible.
- Historical football data must be preserved unless a specific domain lifecycle explicitly permits deletion.

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
