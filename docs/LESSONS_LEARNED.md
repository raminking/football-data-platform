# Lessons Learned

---

## Architecture

### Vertical Slice Architecture

Instead of organizing code only by technical layers, features are organized around use cases.

Example:

Teams

- CreateTeam
- GetTeam
- UpdateTeam
- DeleteTeam

The same approach is used for Competitions, Seasons and Matches.

Benefits

- Better scalability
- Better maintainability
- Easier feature development
- Reduced coupling

---

### Repository Pattern

The Application layer depends on abstractions rather than Entity Framework directly.

Example

Application

`ITeamRepository`, `ICompetitionRepository`, `ISeasonRepository`, `IMatchRepository`

Infrastructure

Concrete repository implementations

Benefits

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

This allows a team to participate in matches across multiple competitions without coupling Match directly to Competition.

### Keep Team Simple

The current MVP Team model intentionally remains:

```text
Team
├── Id
├── Name
└── Country
```

We avoid adding fields such as `ShortName`, `Code`, or a separate Country entity until a real requirement justifies them. This follows KISS/YAGNI and keeps the current domain aligned with the implementation.

### Match MVP Boundary

The first Match model intentionally contains only the information needed for a useful football-data foundation:

- fixture identity
- season
- home/away teams
- scheduled time
- stage
- lifecycle status
- half-time and full-time scores
- result

Detailed match events and competition rules are deliberately postponed to avoid premature complexity.

### Result Consistency

`Result` is not an independent arbitrary fact. It is derived from the final scores.

```text
3 - 1 → HomeWin
1 - 1 → Draw
0 - 2 → AwayWin
```

This prevents contradictory states such as `3 - 1 → Draw`.

### Multiple Foreign Keys to One Entity

Match references Team twice through distinct relationships:

```text
Match.HomeTeamId → Team.Id
Match.AwayTeamId → Team.Id
```

EF Core requires explicit relationship configuration for these two foreign keys. Restricting cascade delete also prevents accidental deletion chains from Team/Season into historical Match records.

---

## Entity Framework Core

Learned

- DbContext configuration
- Entity Configuration
- Fluent API
- Migrations
- Database Update
- PostgreSQL integration testing with Testcontainers
- Explicit configuration for multiple foreign keys from Match to Team
- Keeping persistence configuration separate from domain invariants

---

## PostgreSQL

Selected because

- Excellent performance
- Open Source
- Widely used in Europe
- Excellent EF Core support

---

## Dependency Injection

Application registers MediatR.

Infrastructure registers repositories and DbContext.

API composes the application.

---

## Minimal APIs / Carter

Carter provides a lightweight endpoint model that fits the project's Vertical Slice Architecture.

---

## Testing

Current strategy

- Domain Tests
- Application Tests
- API integration tests
- PostgreSQL integration tests using Testcontainers

Latest verified milestone result:

**59 passed, 0 failed, 0 skipped**

The complete suite remained green after adding Match.

---

## External Data Import — Next Lesson

The next milestone introduces external provider data. Provider-specific DTOs should remain outside the domain so that changing providers does not force domain changes.

The intended boundary is:

```text
External Provider
      ↓
Provider Adapter
      ↓
Internal DTO / Mapping
      ↓
Domain
      ↓
Persistence
```

Idempotency, validation and retry behavior should be designed before introducing recurring background jobs.

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
