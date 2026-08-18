# Football Data Platform

## Vision

Football Data Platform is a production-oriented backend project built to demonstrate modern Backend Engineering and Data Engineering practices.

The project is designed as a portfolio to showcase software architecture, clean code, testing, relational database design, domain modelling, and API development.

---

## Primary Goal

Build a portfolio project capable of demonstrating the skills expected from a Backend Engineer or Data Engineer in Europe.

This project is intentionally developed as if it were a real production system rather than a tutorial application.

---

## Current Status

Current Sprint:

**Sprint 4 – Matches & Results**

Completed milestones:

- **Sprint 2 – Teams Module**
- **Sprint 3 – Competitions & Seasons**

Latest verified suite:

**51 passed, 0 failed, 0 skipped**

Current focus:

- Finalize the Match v1 domain model
- Model Season → Match and Home/Away Team relationships
- Implement match status, stage, scores and result consistency
- CRUD/application operations
- PostgreSQL persistence and migration
- Carter API
- Domain/application/integration tests

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

A Match belongs to a specific Season rather than directly to Competition. This allows the same Team to participate in different competitions and seasons without coupling Match directly to Competition.

---

## Technology Stack

Backend

- ASP.NET Core 8
- C#
- Minimal APIs
- Carter
- MediatR

Database

- PostgreSQL
- Entity Framework Core
- EF Core Migrations

Architecture

- Vertical Slice Architecture
- Clean Architecture
- Repository Pattern
- Rich Domain Model

Testing

- xUnit
- Testcontainers PostgreSQL integration tests

Future

- Docker
- Authentication
- CI/CD
- Logging
- Caching
- Background Jobs
- External football-data synchronization

---

## Development Principles

This project follows:

- Clean Code
- SOLID
- KISS
- YAGNI
- Separation of Concerns
- Production-oriented design
- Explicit domain invariants
- Testable application boundaries

The MVP intentionally avoids premature modelling of competition formats, groups, qualification rules and detailed match events.

---

## Long-Term Goal

When completed, this repository should demonstrate the ability to:

- Design backend systems
- Build REST APIs
- Model real-world domains
- Work with relational databases
- Apply Clean/Vertical Slice Architecture
- Build meaningful automated tests
- Explain architectural decisions
- Integrate external data sources
- Explain engineering trade-offs during Backend Engineer interviews
