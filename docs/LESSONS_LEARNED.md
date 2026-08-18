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

The same approach will be used for Competitions and later football-domain modules.

Benefits

- Better scalability
- Better maintainability
- Easier feature development
- Reduced coupling

---

### Repository Pattern

The Application layer depends on abstractions rather than Entity Framework directly.

Example:

Application

`ITeamRepository`

Infrastructure

`TeamRepository`

The Competition module will follow the same boundary.

Benefits

- Loose coupling
- Easier testing
- Infrastructure can change without affecting business logic

---

## Entity Framework Core

Learned

- DbContext configuration
- Entity Configuration
- Fluent API
- Migrations
- Database Update
- PostgreSQL integration testing with Testcontainers

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

Teams milestone result:

**27 passed, 0 failed, 0 skipped**

The same testing standard will be applied to Competitions.

---

## Project Documentation

`docs/PROJECT_STATE.md` is the operational source of truth for continuing the project across sessions.

Strategic status is maintained in:

- `PROJECT.md`
- `ROADMAP.md`
- `SPRINTS.md`

Architecture decisions are recorded under `docs/ADR/`.
