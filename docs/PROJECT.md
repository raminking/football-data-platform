# Football Data Platform

## Vision

Football Data Platform is a production-oriented backend project built to demonstrate modern Backend Engineering and Data Engineering practices.

The project is designed as a portfolio to showcase software architecture, clean code, testing, relational database design, domain modelling, API development, and external data integration.

---

## Primary Goal

Build a portfolio project capable of demonstrating the skills expected from a Backend Engineer or Data Engineer in Europe.

The project is intentionally developed as a real production-oriented system rather than a tutorial application.

---

## Current Status

**Sprint 5 — Multi-Source External Data & Import Pipeline**

Completed milestones:

- Sprint 1 — Foundation
- Sprint 2 — Teams Module
- Sprint 3 — Competitions & Seasons
- Sprint 4 — Matches & Results
- Sprint 5 — Provider boundary, external identities, Competition/Season/Team/Match imports and end-to-end import orchestration
- Internal/public identifier separation

Latest verified suite:

**102 passed, 0 failed, 0 skipped**

A real local PostgreSQL import has also been verified for Premier League 2025/26: first run `541 created / 0 updated / 0 skipped`, repeated run `0 created / 541 updated / 0 skipped`.

### Current focus

- Transaction and partial-failure semantics
- Source priority/fallback behavior
- Provider error classification, retry/backoff and rate limiting
- Import observability
- Operational readiness before background scheduling

The current domain baseline is documented in `docs/DOMAIN_MODEL.md`.

---

## Core Domain Boundary

```text
Competition
    ↓
Season
    ↓
Match
   ├── HomeTeam
   └── AwayTeam
```

A Match belongs to a specific Season rather than directly to Competition.

---

## Identifier Boundary

```text
Domain persistence:
  Id       → long / PostgreSQL bigint
  PublicId → Guid / PostgreSQL uuid

External source:
  Provider + EntityType + ExternalId
       ↓
  ExternalIdentity
       ↓
  InternalEntityId
```

Internal IDs are optimized for database relationships and joins. Public IDs are used at the API boundary. Provider external IDs never become domain primary keys.

---

## External Data Boundary

```text
Authorized external source
      ↓
FootballDataOrgProvider
      ↓
IFootballDataSource
      ↓
IFootballDataSourceResolver
      ↓
Provider-neutral external records
      ↓
Import Services / Orchestrator
      ↓
ExternalIdentity resolution
      ↓
Domain + PostgreSQL
```

Provider-specific DTOs remain in Infrastructure and do not leak into Domain or API contracts.

Current source:

- `football-data.org` (`SourceKey = football-data.org`)

FotMob is not a production dependency and may only be considered if an authorized/licensed access path becomes available.

---

## Import Pipeline

The current orchestrator coordinates:

```text
Competition → Season → Teams → Matches
```

Import entry point:

```text
POST /imports/{sourceKey}/{competitionCode}/{seasonYear}
```

Example:

```text
POST /imports/football-data.org/PL/2025
```

The response aggregates `Created`, `Updated`, `Skipped`, `Processed` and `Errors` totals.

Imports are idempotent. Existing external identities are resolved and records are updated instead of duplicated.

Status endpoint:

```text
GET /imports/status
```

It reports persisted PostgreSQL counts for competitions, seasons, teams, matches and external identities.

---

## Verified Real-Data Import

Premier League 2025/26 has been exercised against the configured local PostgreSQL database through the import API:

```text
First run:   541 created / 0 updated / 0 skipped
Second run:    0 created / 541 updated / 0 skipped
```

This verifies the current import path is idempotent for that dataset. Actual local database row counts remain runtime state and are not stored in Git.

---

## Technology Stack

### Backend

- ASP.NET Core 8
- C#
- Carter
- MediatR

### Database

- PostgreSQL
- Entity Framework Core
- EF Core Migrations

### Architecture

- Clean Architecture
- Vertical Slice Architecture
- Repository Pattern
- Rich Domain Model
- Provider isolation through application abstractions

### Testing

- xUnit
- Testcontainers PostgreSQL integration tests
- **102 passed, 0 failed, 0 skipped**

---

## Database Initialization

The API applies pending EF Core migrations at startup when the configured PostgreSQL database is reachable.

Manual migration management:

```bash
dotnet ef database update
```

Migrations are located in:

`src/FootballDataPlatform.Infrastructure/Migrations/`

---

## Configuration

`football-data.org` may require an API token depending on the configured provider plan. Configure credentials through User Secrets or environment-specific configuration. Never commit secrets.

The database connection uses the `DefaultConnection` connection string.

---

## Development Verification

```bash
dotnet clean
dotnet build
dotnet test
```

The repository intentionally uses a single `main` branch. Do not create feature branches for this project unless explicitly agreed otherwise.

---

## Development Principles

- Clean Code
- SOLID
- KISS
- YAGNI
- Separation of Concerns
- Production-oriented design
- Explicit domain invariants
- Testable application boundaries
- Provider isolation
- External identity mapping
- Idempotent ingestion before scheduling
- No premature advanced match/event modelling

---

## Next Engineering Focus

1. Review transaction and partial-failure semantics.
2. Verify source priority and safe fallback behavior.
3. Improve provider error classification and diagnostics.
4. Add retry/backoff and rate-limiting strategy where appropriate.
5. Add import observability, health checks and operational diagnostics.
6. Evaluate background scheduling only after synchronous import remains stable.

The long-term goal remains a credible portfolio-quality backend demonstrating architecture, REST APIs, relational data modelling, automated testing and safe external data ingestion.