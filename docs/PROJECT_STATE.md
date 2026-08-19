# Football Data Platform — Project State

> Compact source of truth for continuing the project across sessions.

## Current Milestone
**Sprint 5 — External Data Import: Provider boundary and first provider adapter completed.**

## Completed Milestones
### Teams ✅
- Teams CRUD, domain/application tests, Carter API, PostgreSQL persistence and Testcontainers integration coverage.
- Team metadata support has been introduced for the current requirements.
- Current Team domain remains intentionally focused on core identity plus supported metadata.

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

### Sprint 5 — External Data Provider Boundary ✅
- Provider-independent `IFootballDataProvider` abstraction in Application.
- Provider DTOs kept inside Infrastructure.
- First provider selected: `football-data.org`.
- `FootballDataOrgProvider` implemented with HTTP client integration.
- Provider mapping implemented for competitions, teams and matches.
- Provider options/configuration added.
- API token configuration supported through configuration/user-secrets rather than committed secrets.
- Provider failures are surfaced as HTTP request exceptions at the Infrastructure boundary.
- Changes merged into `main`.

## Current Verification
Latest local verification after the provider adapter work:
- **65 passed**
- **0 failed**
- **0 skipped**
- **65 total**

Build is currently green. The test suite remains the baseline gate for continuing work.

## Current Git State
- `main` is the single source of truth.
- Feature/sprint branches used during the provider work have been removed.
- Latest `main` merge commit: `2a28696` (`merge: football-data.org provider adapter`).
- Working tree is clean and `main` is synchronized with `origin/main`.

## External Provider Boundary
```text
football-data.org
       ↓
FootballDataOrgProvider
       ↓
IFootballDataProvider
       ↓
ExternalCompetition / ExternalTeam / ExternalMatch
       ↓
Import / Mapping layer
       ↓
Domain + Persistence
```

Provider-specific DTOs must not leak into Domain or API contracts.

## Current External Provider Model
```text
IFootballDataProvider
├── GetCompetitionsAsync()
├── GetTeamsAsync(competitionCode, seasonYear)
└── GetMatchesAsync(competitionCode, seasonYear)
```

The current provider adapter maps external identifiers to provider-neutral external records. Persistent external identity and idempotent synchronization are not implemented yet.

## Next Exact Steps — Sprint 5
1. Add provider fixture/adapter tests to the test project and verify the count increases beyond the current 65-test baseline.
2. Introduce persistent external identity for Team, Competition, Season and Match.
3. Define uniqueness around provider + entity type + external identifier.
4. Build import/mapping application services.
5. Implement idempotent upsert/synchronization.
6. Add validation, partial-failure handling and retry strategy.
7. Add end-to-end integration coverage using fixtures/mocks.
8. Only after synchronous import is stable, evaluate background scheduling.

## Important Boundary Decisions
- `main` is the only source of truth for project continuation.
- Provider DTOs stay in Infrastructure.
- Application owns the provider abstraction; Infrastructure owns provider implementations.
- External identifiers must not become domain primary keys.
- Import must be idempotent before scheduling is introduced.
- Do not add advanced match/event modelling merely to mirror a provider response.

## Known Issues / Decisions To Review
- Teams update endpoint remains `POST /teams/update`; route consistency can be refactored later.
- Competition-to-Team relationships remain deferred except where required by Match relationships.
- Optimistic concurrency, soft delete and other advanced production concerns remain deferred until justified.
- Provider rate limiting/backoff and richer error classification remain to be designed as part of the import workflow.

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
