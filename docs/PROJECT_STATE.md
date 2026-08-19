# Football Data Platform — Project State

> Compact source of truth for continuing the project across sessions.

## Current Milestone
**Sprint 5 — External Data Import: Team synchronization completed and Competition import persistence coverage added.**

## Completed Milestones
### Teams ✅
- Teams CRUD, domain/application tests, Carter API, PostgreSQL persistence and Testcontainers integration coverage.
- Team metadata support has been introduced for the current requirements.
- Current Team domain remains intentionally focused on core identity plus supported metadata.
- Application-level Team import service implemented.
- Team import resolves provider external identity and creates or updates the internal Team.
- Repeated imports are idempotent.
- PostgreSQL integration coverage verifies Team persistence and ExternalIdentity persistence across repeated imports.

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

### Sprint 5 — External Data Provider & Import 🚧
- Provider-independent `IFootballDataProvider` abstraction in Application.
- Provider DTOs kept inside Infrastructure.
- First provider selected: `football-data.org`.
- `FootballDataOrgProvider` implemented with HTTP client integration.
- Provider mapping implemented for competitions, teams and matches.
- Provider options/configuration added.
- API token configuration supported through configuration/user-secrets rather than committed secrets.
- Provider failures are surfaced as HTTP request exceptions at the Infrastructure boundary.
- Deterministic provider adapter tests added using fake HTTP handlers and JSON fixtures.
- Persistent `ExternalIdentity` support added for provider/entity/external-id mapping.
- Unique `(Provider, EntityType, ExternalId)` constraint added through EF Core migration.
- External identity repository abstraction and PostgreSQL implementation registered in DI.
- Team import service implemented and registered in DI.
- Team import unit coverage added.
- Team import PostgreSQL integration coverage added.
- Repeated Team import verified as idempotent: existing external identity is reused and the Team is updated without creating duplicates.
- Competition import application service implemented.
- Competition import PostgreSQL integration coverage added for create, update and idempotent external-identity persistence.
- Changes are merged into `main`.

## Current Verification
Latest local verification:
- **86 passed**
- **0 failed**
- **0 skipped**
- **86 total**

Full test suite is green. The 86-test baseline is the current gate for continuing work.

## Current Git State
- `main` is the single source of truth.
- Feature/test branches used during Sprint 5 have been removed.
- `origin/main` is the source of truth for continuation.
- Working tree was clean at the last reported verification.

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
ExternalIdentity
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

The provider adapter maps external identifiers to provider-neutral external records. Persistent external identity and Team/Competition import workflows are implemented.

## Import Workflows
### Team
```text
IFootballDataProvider
       ↓
TeamImportService
       ↓
Find ExternalIdentity
   ┌───┴───┐
   │       │
 found   missing
   │       │
 update  create Team
   │       │
   └───┬───┘
       ↓
Persist ExternalIdentity
       ↓
PostgreSQL
```

The workflow is idempotent for repeated imports of the same provider external identifier.

### Competition
The Competition import follows the same provider-neutral identity pattern: resolve `(Provider, EntityType, ExternalId)`, create when missing, update when present, and prevent duplicate domain records when an equivalent competition already exists.

## External Identity Boundary
```text
ExternalIdentity
├── Provider
├── EntityType
├── ExternalId
└── EntityId
```

The database enforces uniqueness for `(Provider, EntityType, ExternalId)`. External identifiers remain integration identities and do not become domain primary keys.

## Current Task
Finish the persistence and synchronization boundary for Competition, then implement Season import with correct Competition resolution and external identity handling.

## Next Exact Steps — Sprint 5
1. Add an integration test proving duplicate ExternalIdentity persistence is rejected by the database constraint.
2. Review Team and Competition import transaction/error behavior and partial-failure semantics.
3. Complete Competition import unit/idempotency coverage where gaps remain.
4. Implement Season import/mapping, resolving its Competition relationship and external identity.
5. Add Season create/update/idempotency unit and PostgreSQL integration coverage.
6. Add validation, partial-failure handling and provider error classification.
7. Extend the workflow to Match after Competition/Season synchronization is stable.
8. Add end-to-end import integration coverage using provider fixtures/mocks.
9. Only after synchronous import is stable, evaluate background scheduling and retry/backoff.

## Important Boundary Decisions
- `main` is the only source of truth for project continuation.
- Provider DTOs stay in Infrastructure.
- Application owns provider and persistence abstractions; Infrastructure owns implementations.
- External identifiers must not become domain primary keys.
- External identity is unique by provider + entity type + external identifier.
- Import must be idempotent before scheduling is introduced.
- Do not add advanced match/event modelling merely to mirror a provider response.

## Known Issues / Decisions To Review
- Teams update endpoint remains `POST /teams/update`; route consistency can be refactored later.
- Competition-to-Team relationships remain deferred except where required by Match relationships.
- Optimistic concurrency, soft delete and other advanced production concerns remain deferred until justified.
- Provider rate limiting/backoff and richer error classification remain to be designed as part of the import workflow.
- Season external identity must be resolved as part of the import workflow even though the current provider abstraction exposes seasons through competition/season context rather than a separate `GetSeasonsAsync` operation.

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
