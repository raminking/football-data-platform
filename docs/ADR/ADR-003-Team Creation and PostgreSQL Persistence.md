# ADR-003: Team Creation Vertical Slice and PostgreSQL Persistence

## Status

Accepted

## Date

2026-08-13

## Context

The project needed its first complete business feature in order to validate the selected architecture and database strategy.

The first feature selected was Team creation.

The feature required:

- A domain entity
- Domain validation
- An application use case
- Persistence abstraction
- Persistence implementation
- Database schema
- HTTP API endpoint
- Automated tests

The project also needed a clear separation between application logic and database implementation.

---

## Decision

The Team feature will be implemented as a vertical slice.

The feature follows this flow:

```text
HTTP Request
    ↓
CreateTeamEndpoint
    ↓
CreateTeamCommand
    ↓
CreateTeamHandler
    ↓
ITeamRepository
    ↓
TeamRepository
    ↓
FootballDataDbContext
    ↓
Entity Framework Core
    ↓
PostgreSQL