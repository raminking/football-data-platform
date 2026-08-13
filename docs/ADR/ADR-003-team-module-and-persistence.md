
---

### 5. `docs/ADR/ADR-003-team-module-and-persistence.md`

Create this new file:

```markdown
# ADR-003: Team Creation Vertical Slice and PostgreSQL Persistence

## Status

Accepted

## Date

2026-08-13

## Context

The project needed its first complete business feature in order to validate the selected architecture and database strategy.

The first feature selected was team creation.

The feature required:

- A domain entity
- Business validation
- An application use case
- Persistence abstraction
- Persistence implementation
- Database schema
- HTTP API endpoint
- Automated tests

The project also needed a clear separation between application logic and database implementation.

---

## Decision

We will implement the Team feature as a vertical slice.

The feature will follow this flow:

```text
HTTP Request
    ↓
API Endpoint
    ↓
MediatR Command
    ↓
Command Handler
    ↓
ITeamRepository
    ↓
TeamRepository
    ↓
Entity Framework Core
    ↓
PostgreSQL