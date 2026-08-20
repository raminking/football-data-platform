# Sprint History

---

# Sprint 1 — Project Foundation

**Status: ✅ Completed**

- Created solution
- Established project structure
- Adopted Vertical Slice Architecture
- Configured Dependency Injection
- Configured PostgreSQL
- Configured Entity Framework Core
- Created initial migration

---

## Sprint 2: Teams Module

**Status: ✅ Completed and locally verified**

- Full Team CRUD
- Domain validation and uniqueness
- Repository + PostgreSQL persistence
- Carter API
- MediatR/CQRS
- Testcontainers integration coverage

### Historical Verification
- **27 passed, 0 failed, 0 skipped**

---

## Sprint 3: Competitions & Seasons

**Status: ✅ Completed and locally verified**

- Competition CRUD and validation
- Season entity and Competition relationship
- Season date-range validation
- Unique season name within competition
- PostgreSQL persistence and EF migration
- Carter API
- Domain/application/integration coverage

### Historical Verification
- **51 passed, 0 failed, 0 skipped**

---

## Sprint 4: Matches & Results

**Status: ✅ Completed and locally verified**

- Match v1 entity and enums
- Season/HomeTeam/AwayTeam relationships
- Match status lifecycle and stage
- Full-time and half-time scores
- Result derived from final scores
- Domain invariants
- Application CRUD
- Repository + PostgreSQL implementation
- EF Core configuration/migration
- Carter API and contracts
- Automated test coverage
- Documentation synchronization

### Historical Verification
- **59 passed, 0 failed, 0 skipped**

### Deliberately Deferred
- Extra-time and penalty-shootout scores
- Goals, cards, substitutions and detailed events
- Lineups, referee, venue and weather
- Competition formats/groups/qualification rules
- Season participants and promotion/relegation rules

---

## Sprint 5: Multi-Source External Data & Import

**Status: 🚧 In progress**

**Goal:** Build a provider-independent ingestion boundary and a safe, idempotent synchronization pipeline that can support multiple authorized football-data sources.

### Completed Stories
- [x] First external provider: `football-data.org`.
- [x] Provider-specific DTOs isolated in Infrastructure.
- [x] Provider-neutral `ExternalCompetition`, `ExternalSeason`, `ExternalTeam`, `ExternalMatch` records.
- [x] `IFootballDataSource` abstraction.
- [x] `IFootballDataSourceResolver` abstraction.
- [x] `FootballDataOrgProvider` implements the source contract and exposes `SourceKey = "football-data.org"`.
- [x] Resolver-based source selection using case-insensitive source keys.
- [x] Persistent `ExternalIdentity` mapping provider/entity/external-id to internal entities.
- [x] Database uniqueness for `(Provider, EntityType, ExternalId)`.
- [x] Competition, Season and Team import services migrated to the source-neutral resolver boundary.
- [x] Idempotent import behavior covered by tests.
- [x] Provider adapter tests with deterministic HTTP handlers/fixtures.
- [x] PostgreSQL integration coverage for persistence and repeated imports.
- [x] Latest user-verified test suite: **97 passed, 0 failed, 0 skipped**.

### Important Source Decision
- `football-data.org` is the current official/authorized source used by the project.
- FotMob is **not** treated as a production source. It may only be added if an authorized/licensed access path is available. The project must not depend on unauthorized scraping or private reverse-engineered endpoints.

### Current Task
- [ ] Complete source priority/fallback behavior.
- [ ] Add resolver tests for registered source, case-insensitive lookup, unknown source and empty key.
- [ ] Complete Match import through `IFootballDataSource`.
- [ ] Add deterministic end-to-end import coverage.
- [ ] Execute and verify a real `football-data.org` import into the configured PostgreSQL database.
- [ ] Define transaction and partial-failure behavior.

### Next Stories
- [ ] Provider priority and safe fallback semantics.
- [ ] Match import and ExternalIdentity resolution for competition/season/teams.
- [ ] Real-data import verification and database record counts.
- [ ] Retry/backoff and provider error classification.
- [ ] Rate limiting and provider health handling.
- [ ] Background scheduling only after synchronous import is stable.

### Database Status
PostgreSQL persistence and migrations are implemented, but **live football data in the user's local database has not yet been verified**. Integration-test data is not considered production/live source data. The next milestone must explicitly run an import and verify persisted records.

### Sprint Outcome So Far
The project has moved from a single-provider import design to a source-neutral Multi-Source architecture. Application and Domain do not depend on a concrete provider. External identities provide stable source-scoped mapping and idempotency. The next focus is completing the import pipeline and proving real database ingestion.

---

## Sprint 6: Production Readiness

**Status: Planned**

- Authentication and Authorization
- Structured Logging
- Docker Containerization
- Health Checks
- Advanced CI/CD
- Security Scans
- Performance Tests
