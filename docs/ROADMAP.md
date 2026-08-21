# Roadmap

This document outlines the strategic development plan for the Football Data Platform.

The project is intentionally built as a **football analytics data platform**, not only as a CRUD API for one provider. We implement capabilities incrementally while keeping the domain extensible.

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
- Players, lineups and availability
- Match officials
- Formation/tactical context
- Venue and weather
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

## Sprint 5.5 — Extensible Football Domain Blueprint
**Status:** Architecture decisions documented; implementation intentionally deferred

The platform now has an explicit extension direction for football analytics without requiring a big-bang implementation.

### Domain concepts reserved for incremental implementation
- [ ] Player as an independent entity
- [ ] Player retirement (`RetiredAt`) without deleting historical data
- [ ] Player ↔ Position many-to-many capability model
- [ ] Historical Player ↔ Team assignment
- [ ] MatchTeam context
- [ ] Actual MatchLineup: starters and substitutes only
- [ ] Predicted lineup kept separate from actual lineup
- [ ] Player availability for a Match: injury, suspension, doubtful, illness, coach decision, etc.
- [ ] Independent Coach and historical TeamCoachAssignment
- [ ] Formation catalog
- [ ] TeamTacticalProfile with historical validity
- [ ] Match-level predicted/starting/tactical formations
- [ ] Match officials and role assignments
- [ ] First-class MatchEvent model
- [ ] Later derived match/season statistics

### Architectural principles
- Keep the current core small; add concepts only when justified by a real use case or source.
- Do not put historical relationships into current-state columns.
- Do not merge predicted facts with actual facts.
- Do not model a player's normal position as their match position.
- Do not copy a full team roster into every Match.
- Do not turn every provider field into a domain field.
- Do not create generic blobs where a real structured domain concept is required.
- Preserve historical data; retirement and team departure are not deletion.
- Keep provider-specific DTOs outside Domain/Application contracts.

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

---

## Longer-Term Analytics Direction

The following are intentionally future capabilities rather than immediate implementation requirements:

```text
Master Data
  Player / Team / Competition / Season / Official
             ↓
Match Context
  Lineup / Availability / Formation / Officials
             ↓
Raw Events
  Goals / Cards / Substitutions / Other Events
             ↓
Derived Statistics
  Player Match / Team Match / Player Season / Team Season
             ↓
Analytics / BI / ML
```

A separate analytical/OLAP layer may be introduced only when scale and use cases justify it. The transactional PostgreSQL model should not be prematurely turned into a data warehouse.
