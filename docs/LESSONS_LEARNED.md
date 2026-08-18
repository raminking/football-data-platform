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

The same approach is used for Competitions, Seasons and later football-domain modules such as Matches.

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

`ITeamRepository`

Infrastructure

`TeamRepository`

The same boundary is used for Competition, Season and Match persistence.

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

This avoids putting year-specific information on Competition and allows the same competition to change its participating-team count or format between seasons.

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

`Result` is not an independent arbitrary fact. It must agree with the final scores.

For example:

```text
3 - 1 → HomeWin
1 - 1 → Draw
0 - 2 → AwayWin
```

An inconsistent state such as `3 - 1 → Draw` must be rejected by the domain/application model.

Where practical, Result should be derived from the final scores.

---

## Entity Framework Core

Learned

- DbContext configuration
- Entity Configuration
- Fluent API
- Migrations
- Database Update
- PostgreSQL integration testing with Testcontainers
- Multiple foreign keys from Match to the same Team entity require explicit relationship configuration

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

**51 passed, 0 failed, 0 skipped**

The same verification standard is required for Match before the milestone can be considered complete.

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
