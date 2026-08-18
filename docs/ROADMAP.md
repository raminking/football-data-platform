# Roadmap

This document outlines the strategic development plan for the Football Data Platform.

---

## Sprint 1 — Foundation ✅
**Status:** Completed

- Solution structure
- Clean Architecture + Vertical Slice Architecture
- PostgreSQL & Entity Framework Core setup
- Dependency Injection configuration
- Initial Migration
- CI/CD Pipeline basics

---

## Sprint 2 — Teams Module ✅
**Status:** Completed and locally verified

**Completed Features:**
- [x] Team Entity (Rich Domain Model)
- [x] Create/Get/Update/Delete Team
- [x] Validation and uniqueness rules
- [x] Repository implementation
- [x] Carter API
- [x] PostgreSQL/Testcontainers integration tests
- [x] Full suite verified: 27 passed, 0 failed, 0 skipped

**Deferred:**
- [ ] Optimistic Concurrency Control
- [ ] Soft Delete vs Hard Delete ADR
- [ ] Endpoint route consistency

---

## Sprint 3 — Competitions & Seasons ✅
**Status:** Completed and locally verified

### Competition
- [x] Competition Entity
- [x] CRUD
- [x] Validation and uniqueness
- [x] PostgreSQL persistence
- [x] Carter API
- [x] Domain/integration tests
- [x] EF Core migration

### Season
- [x] Season Entity
- [x] Competition → Season relationship
- [x] Date-range validation
- [x] Unique season name within competition
- [x] CRUD
- [x] PostgreSQL persistence
- [x] Carter API
- [x] Integration coverage
- [x] Verified baseline: 51 passed, 0 failed, 0 skipped

---

## Sprint 4 — Matches & Results ✅
**Status:** Completed and locally verified

### Completed
- [x] Match v1 domain model
- [x] Season → Match relationship
- [x] HomeTeam/AwayTeam relationships
- [x] Match Status lifecycle
- [x] Match Stage model
- [x] Full-time and half-time scores
- [x] Result derived from final scores
- [x] Domain invariants
- [x] Application CRUD
- [x] Repository abstraction and PostgreSQL implementation
- [x] EF Core configuration and migration
- [x] Carter API
- [x] Domain/application/integration test coverage
- [x] Full local suite verification
- [x] Documentation synchronization

### Match v1 Model

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

### Verification
**59 passed, 0 failed, 0 skipped**

### Intentionally Deferred
- Extra-time and penalty-shootout score modelling
- Goals, cards, substitutions and other match events
- Lineups, referee, venue and weather
- Competition format/rules subsystem
- Groups and season participants
- Promotion/relegation and qualification rules

---

## Sprint 5 — External Data Import 🚧
**Status:** Next

**Goal:** Introduce a provider-independent ingestion boundary for real football data.

### Planned Work
- [ ] Select and document first football-data provider
- [ ] Define external provider adapter abstraction
- [ ] Keep provider DTOs separate from internal domain models
- [ ] Map provider data to Team, Competition, Season and Match
- [ ] Implement validation and idempotent synchronization
- [ ] Define import error handling and retry strategy
- [ ] Add provider fixtures/mocked integration tests
- [ ] Evaluate background scheduling after import flow is stable

### Engineering Focus
The import layer must not leak provider-specific models into the domain. Provider changes should be isolated behind an adapter boundary.

---

## Sprint 6 — Production Readiness
**Status:** Planned

- Authentication (JWT) & Authorization
- Structured Logging (Serilog)
- Docker Containerization
- Health Checks
- Advanced CI/CD
- Security Scans
- Performance Tests
