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

- [x] Team entity and domain validation
- [x] Create/Get/Update/Delete Team
- [x] Repository implementation
- [x] Carter API
- [x] PostgreSQL/Testcontainers integration tests

---

## Sprint 3 — Competitions & Seasons ✅
**Status:** Completed and locally verified

- [x] Competition CRUD and validation
- [x] Season entity and Competition relationship
- [x] Season date-range validation
- [x] Unique season name within competition
- [x] PostgreSQL persistence and EF migration
- [x] Carter API
- [x] Integration coverage

---

## Sprint 4 — Matches & Results ✅
**Status:** Completed and locally verified

- [x] Match v1 domain model
- [x] Season/HomeTeam/AwayTeam relationships
- [x] Match Status lifecycle and Stage
- [x] Full-time and half-time scores
- [x] Result derived from final scores
- [x] Domain invariants
- [x] Application CRUD
- [x] Repository + PostgreSQL implementation
- [x] EF Core configuration/migration
- [x] Carter API and contracts
- [x] Full local test coverage

### Intentionally Deferred
- Extra-time and penalty-shootout score modelling
- Goals, cards, substitutions and other match events
- Lineups, referee, venue and weather
- Competition format/rules subsystem
- Groups, season participants and promotion/relegation rules

---

## Sprint 5 — Multi-Source External Data & Import
**Status:** Core pipeline completed and locally verified

### Completed
- [x] `football-data.org` source adapter
- [x] Provider-neutral `IFootballDataSource` abstraction
- [x] `IFootballDataSourceResolver`
- [x] Provider DTO isolation in Infrastructure
- [x] Provider-neutral external records
- [x] Persistent `ExternalIdentity` mapping
- [x] Unique `(Provider, EntityType, ExternalId)` constraint
- [x] Competition import
- [x] Season import
- [x] Team import
- [x] Match import
- [x] End-to-end `FootballDataImportOrchestrator`
- [x] Import API: `POST /imports/{sourceKey}/{competitionCode}/{seasonYear}`
- [x] Import status API: `GET /imports/status`
- [x] Idempotent repeated imports
- [x] Internal `long` IDs separated from public `Guid` IDs
- [x] Full suite: **102 passed, 0 failed, 0 skipped**
- [x] Real PostgreSQL verification for Premier League 2025/26: `541 created` first run, `541 updated` second run

### Remaining Engineering Work
- [ ] Source priority and safe fallback semantics
- [ ] Transaction and partial-failure behavior
- [ ] Provider error classification and diagnostics
- [ ] Retry/backoff and rate limiting
- [ ] Import observability

### Source Decision
`football-data.org` is the current authorized source. FotMob is not a production dependency and may only be considered if an authorized/licensed access path becomes available.

---

## Sprint 6 — Production Readiness
**Status:** Planned

- Authentication & Authorization
- Structured Logging
- Docker Containerization
- Health Checks
- Advanced CI/CD
- Security Scans
- Performance Tests
- Import retries/rate limiting
- Operational metrics and diagnostics
- Background scheduling after synchronous ingestion is stable
