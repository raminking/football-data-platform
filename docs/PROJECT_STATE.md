# Football Data Platform — Project State

> This file is the compact source of truth for continuing the project across sessions.

## Current Milestone
Teams module — CRUD API and automated testing foundation.

## Architecture
- Domain
- Application
- Infrastructure
- API
- Contracts
- Tests
- CQRS/MediatR in Application
- Carter endpoints in API
- EF Core + PostgreSQL in Infrastructure

## Completed
- Team domain entity with validation.
- CreateTeam command and handler.
- GetTeam query and handler.
- UpdateTeam command and handler.
- DeleteTeam command and handler.
- Team repository and persistence.
- PostgreSQL database with EF Core migrations.
- Unique Team + Country database constraint.
- API contracts for Teams:
  - CreateTeamRequest
  - CreateTeamResponse
  - GetTeamResponse
- Carter-based Teams API endpoints.
- Application unit tests for CreateTeam, GetTeam, UpdateTeam, DeleteTeam.
- Domain tests for Team.
- Tests organized by feature under `tests/FootballDataPlatform.Tests/Teams`.
- PostgreSQL Testcontainers integration-test infrastructure.
- `CustomWebApplicationFactory` starts PostgreSQL, injects its connection string, and applies EF migrations.
- CreateTeam integration coverage: valid request, empty name, empty country, duplicate Team + Country.
- GetTeam integration coverage: existing team with response validation, unknown ID.
- UpdateTeam integration coverage: valid update, unknown ID, empty name, empty country, duplicate Team + Country.
- DeleteTeam integration coverage: existing team returns `204` and is no longer retrievable; unknown ID returns `404`.

## Test Organization
```text
Teams/
├── Application/
│   ├── CreateTeam/
│   ├── DeleteTeam/
│   ├── GetTeam/
│   └── UpdateTeam/
├── Domain/
└── Integration/
```

## Last Verified Locally
`dotnet test`

Result: **15 passed, 0 failed, 0 skipped** was the last locally verified result recorded before the latest integration-test additions.

The complete Teams integration suite has now been committed to `main`, but it has **not** been executed from this session because the GitHub connector cannot run the local .NET test suite.

Integration tests require Docker Desktop to be running because Testcontainers starts PostgreSQL containers.

## Current Task
Teams CRUD implementation and integration coverage are complete from a code perspective. The remaining gate is local test verification before moving to the next module.

## Next Exact Steps
1. Run `dotnet test` locally with Docker Desktop running.
2. If tests pass, record the exact result here and close the Teams milestone.
3. If tests fail, fix the failures before proceeding.
4. After Teams is verified, move to the next football-data domain module defined by the project roadmap.
5. Keep `main` as the source of truth; use short-lived branches only when needed.

## Success Condition For Current Milestone
Teams CRUD has meaningful API-level integration coverage against a real PostgreSQL container, with all tests passing locally.

## Known Issues
- Docker Desktop must be running for Testcontainers integration tests.
- The complete latest integration suite has not yet been locally verified in this session.
- Update endpoint currently uses `POST /teams/update` rather than a conventional `PUT /teams/{id}` route. This is existing API design and should be reviewed before treating the Teams API as final.

## Important Decisions
- Organize tests by business feature first (`Teams`), then by test type (`Application`, `Domain`, `Integration`).
- Keep request/response DTOs in `FootballDataPlatform.Contracts` instead of exposing Application commands directly from API endpoints.
- Use Testcontainers PostgreSQL for integration tests instead of depending on a developer's local PostgreSQL instance.
- Keep the project state in this file rather than relying on the chat history.
- `docs/PROJECT_STATE.md` must be updated after every meaningful project step so progress survives conversation/branch cleanup.

## Session Protocol
At the start of a new session:

```bash
git status
git log -5 --oneline
cat docs/PROJECT_STATE.md
dotnet test
```

If `dotnet test` fails, capture only the relevant failure and update this file after the issue is resolved.

After each meaningful milestone, update:
- Current Task
- Last Verified Locally
- Next Exact Steps
- Known Issues
- Completed
