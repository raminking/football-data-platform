# Lessons Learned

---

## Architecture

### Vertical Slice Architecture

Features are organized around use cases rather than only technical layers. The same approach is used for Teams, Competitions, Seasons, Matches and import workflows.

Benefits:

- Better scalability
- Better maintainability
- Easier feature development
- Reduced coupling

### Repository Pattern

Application depends on repository abstractions while Infrastructure owns Entity Framework implementations.

Benefits:

- Loose coupling
- Easier testing
- Infrastructure can change without affecting business logic

---

## Domain Modelling

### Competition vs Season

A Competition represents the competition itself, while a Season represents a specific edition.

```text
Competition
└── Season
```

### Match Belongs to Season

A Match belongs to a specific Season rather than directly to Competition.

```text
Competition
└── Season
    └── Match
        ├── HomeTeam
        └── AwayTeam
```

### Keep Team Simple

The MVP intentionally avoids fields and concepts that are not required by current use cases.

### Match MVP Boundary

The first Match model contains fixture identity, season, home/away teams, scheduled time, stage, lifecycle status, half-time/full-time scores and result. Detailed events and competition rules remain deferred.

### Result Consistency

`Result` must agree with final scores:

```text
3 - 1 → HomeWin
1 - 1 → Draw
0 - 2 → AwayWin
```

This prevents contradictory states.

---

## Identifier Design

A major migration separated internal database identifiers from public API identifiers:

```text
Internal Id  → long / bigint → database relationships
PublicId     → Guid / uuid   → API boundary
```

This allows efficient relational keys without exposing database identifiers as the public contract.

External provider IDs are a third category. They are stored in `ExternalIdentity` together with the source and entity type and never become domain primary keys.

---

## External Data Import

Provider-specific DTOs must remain inside Infrastructure. Application consumes provider-neutral records through `IFootballDataSource` and selects sources through `IFootballDataSourceResolver`.

The current pipeline is:

```text
Source
  ↓
IFootballDataSource
  ↓
Import Orchestrator
  ↓
Competition → Season → Team → Match
  ↓
ExternalIdentity
  ↓
Domain + PostgreSQL
```

### Idempotency

External identity uniqueness on:

```text
(Provider, EntityType, ExternalId)
```

provides the persistence foundation for idempotent synchronization.

A real Premier League 2025/26 import was verified locally:

```text
First run:   541 created / 0 updated / 0 skipped
Second run:    0 created / 541 updated / 0 skipped
```

This is stronger evidence than integration-test fixtures alone because the full import path was exercised against local PostgreSQL.

### Public IDs at the API Boundary

API integration tests must deserialize the API's `PublicId`, not assume that a response field named `Id` contains the internal database key. This distinction is important when internal and public identifiers are intentionally separated.

### Import Before Scheduling

Import idempotency should be proven before adding recurring background jobs. Scheduling amplifies every weakness in retries, duplicate handling and partial-failure semantics.

---

## Entity Framework Core

Learned:

- DbContext configuration
- Entity Configuration
- Fluent API
- Migrations
- Database Update
- PostgreSQL integration testing with Testcontainers
- Explicit configuration for multiple foreign keys from Match to Team
- Keeping persistence configuration separate from domain invariants
- Maintaining migration Designer files and model snapshots together
- Carefully synchronizing schema changes when changing identifier types

---

## PostgreSQL

Selected because:

- Excellent performance
- Open source
- Widely used in Europe
- Excellent EF Core support

Testcontainers provides isolated PostgreSQL infrastructure for integration tests.

---

## Dependency Injection

Application registers use cases and MediatR. Infrastructure registers repositories, DbContext and external providers/resolvers. API composes the application.

---

## Minimal APIs / Carter

Carter provides a lightweight endpoint model that fits the project's Vertical Slice Architecture.

---

## Testing

Current strategy:

- Domain tests
- Application tests
- API integration tests
- PostgreSQL integration tests using Testcontainers
- Provider adapter tests with deterministic HTTP handlers/fixtures

Latest verified result:

**102 passed, 0 failed, 0 skipped, 102 total**

---

## Current Engineering Lessons

The next risks are no longer basic CRUD or import functionality. The important production concerns are:

- Transaction and partial-failure semantics
- Source priority and safe fallback
- Provider error classification
- Retry/backoff
- Rate limiting
- Import observability
- Health checks and operational diagnostics

These should be addressed before background scheduling.

---

## Project Documentation

`docs/PROJECT_STATE.md` is the operational source of truth for continuing the project across sessions.

The current domain baseline is maintained in:

- `docs/DOMAIN_MODEL.md`

Strategic status is maintained in:

- `PROJECT.md`
- `ROADMAP.md`
- `SPRINTS.md`

Architecture decisions are recorded under `docs/ADR/`.
