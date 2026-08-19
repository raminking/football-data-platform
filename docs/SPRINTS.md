# Sprint History

---

# Sprint 1 — Project Foundation

Status

✅ Completed

Completed

- Created solution
- Established project structure
- Adopted Vertical Slice Architecture
- Configured Dependency Injection
- Configured PostgreSQL
- Configured Entity Framework Core
- Created initial migration

Outcome

A production-oriented project foundation was established.

---

## Sprint 2: Teams Module

**Status: ✅ Completed and locally verified**

**Goal:** Implement full CRUD operations for the Teams aggregate.

### Completed Stories
- [x] **Create Team:** Register a new football team with name and country.
- [x] **Get Team:** Retrieve details of a specific team by ID.
- [x] **Update Team:** Update team details with uniqueness and domain rules.
- [x] **Delete Team:** Remove a team from the system.

### Technical Implementation
- Vertical Slice Architecture with Clean Architecture principles.
- CQRS with MediatR.
- Rich Domain Model.
- Repository abstraction in Application and implementation in Infrastructure.
- Carter API endpoints.
- PostgreSQL persistence with EF Core.
- Testcontainers PostgreSQL integration tests.

### Historical Verification
- **27 passed, 0 failed, 0 skipped**

### Deferred / Future Improvements
- [ ] Optimistic Concurrency Control.
- [ ] Soft Delete vs Hard Delete ADR.
- [ ] Review update endpoint route consistency.

---

## Sprint 3: Competitions & Seasons

**Status: ✅ Completed and locally verified**

**Goal:** Build the competition and season domain required for the football platform.

### Completed Stories
- [x] **Competition Entity:** Support league and cup competitions.
- [x] **Create/Get/Update/Delete Competition:** Validated competition identity and lifecycle operations.
- [x] **Season Entity:** Model a specific competition edition.
- [x] **Competition → Season relationship:** Establish the relationship with validation.
- [x] **Season date-range validation.**
- [x] **Unique season name within competition.**
- [x] **PostgreSQL persistence and EF migration.**
- [x] **Carter API.**
- [x] **Domain/Application/integration coverage.**

### Historical Verification
- **51 passed, 0 failed, 0 skipped**

---

## Sprint 4: Matches & Results

**Status: ✅ Completed and locally verified**

**Goal:** Implement the core Match domain that connects Teams to a specific Competition Season.

### Completed Stories
- [x] Match v1 entity and enums.
- [x] Season → Match relationship.
- [x] HomeTeam and AwayTeam relationships.
- [x] Match status lifecycle.
- [x] Simple Match stage model.
- [x] Full-time and half-time scores.
- [x] Result derived from final scores.
- [x] Domain invariants and validation.
- [x] Application CRUD operations.
- [x] Repository abstraction and PostgreSQL implementation.
- [x] EF Core configuration and migration.
- [x] Carter API endpoints.
- [x] Match contracts.
- [x] Automated test coverage.
- [x] Full local test suite verification.
- [x] Documentation synchronization.

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

### Historical Verification
- **59 passed, 0 failed, 0 skipped**

### Deliberately Deferred
- [ ] Extra-time and penalty-shootout scores.
- [ ] Goals, cards, substitutions and detailed match events.
- [ ] Lineups, referee, venue and weather.
- [ ] Competition formats, groups and qualification rules.
- [ ] Season participants and promotion/relegation rules.

---

## Sprint 5: External Data Import

**Status: 🚧 In progress**

**Goal:** Introduce a production-oriented external football-data ingestion boundary without coupling providers directly to the domain, then build safe idempotent synchronization.

### Completed Stories
- [x] Select first external football-data provider: `football-data.org`.
- [x] Define provider-independent `IFootballDataProvider` abstraction.
- [x] Create provider-specific DTOs inside Infrastructure.
- [x] Implement `FootballDataOrgProvider`.
- [x] Register provider options and typed HTTP client.
- [x] Map provider competitions, teams and matches into provider-neutral external records.
- [x] Add deterministic provider adapter tests using fake HTTP handlers and JSON fixtures.
- [x] Introduce persistent external identity for provider/entity/external-id mapping.
- [x] Add database uniqueness for `(Provider, EntityType, ExternalId)`.
- [x] Add external identity repository abstraction and PostgreSQL implementation.
- [x] Implement Team import/mapping application service.
- [x] Implement idempotent Team synchronization using ExternalIdentity.
- [x] Add Team import unit coverage.
- [x] Add PostgreSQL integration coverage for Team persistence and repeated imports.
- [x] Verify current suite: **81 passed, 0 failed, 0 skipped**.

### Current Task
- [ ] Add an integration test proving duplicate ExternalIdentity persistence is rejected by the database constraint.
- [ ] Review Team import transaction/error behavior and partial-failure semantics.

### Next Stories
- [ ] Implement Competition import/mapping service.
- [ ] Implement Competition create/update/idempotency unit and integration coverage.
- [ ] Implement Season import/mapping, including Competition relationship resolution.
- [ ] Extend import to Match.
- [ ] Define validation and partial-failure handling.
- [ ] Define retry/backoff and provider error classification.
- [ ] Add end-to-end import integration coverage.
- [ ] Evaluate background job scheduling only after synchronous import is stable.

### Sprint Outcome So Far
The provider boundary, deterministic adapter tests, persistent external identity, and first idempotent Team synchronization workflow are complete. Sprint 5 is now moving from Team synchronization hardening to Competition and Season synchronization.

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
