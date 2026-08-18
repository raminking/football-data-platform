# Football Data Platform — Project State

> Compact source of truth for continuing the project across sessions.

## Current Milestone
Competition module — domain and CRUD foundation.

## Previous Milestone — Teams
**Completed and locally verified:** `27 passed, 0 failed, 0 skipped`.

Teams CRUD, unit tests, domain tests, and PostgreSQL Testcontainers integration coverage are complete.

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

## Competition Module Goal
Introduce competitions/leagues as a first-class football domain concept that can later own seasons, teams, standings, fixtures, and other competition-specific data.

### Initial Scope
- Competition domain entity
- Name
- Country
- Competition code/short code
- Basic validation
- Create/Get/Update/Delete application operations
- Repository + EF Core persistence
- API contracts and Carter endpoints
- Unit tests
- PostgreSQL integration tests

### Out of Scope For This Step
- Seasons
- Competition-team membership
- Standings
- Fixtures/matches
- External football-data provider synchronization
- Advanced competition metadata

## Current Task
Design and implement the Competition module following the proven Teams architecture, without introducing relationships to Teams yet.

## Next Exact Steps
1. Inspect existing project conventions and determine the final Competition fields from the current domain/roadmap.
2. Implement Competition domain entity and tests.
3. Implement Create/Get/Update/Delete application handlers and unit tests.
4. Add EF Core configuration, repository, and migration.
5. Add API contracts and Carter endpoints.
6. Add integration tests for CRUD and validation.
7. Run `dotnet test` locally and record the exact result here.
8. Update any relevant documentation/README files if the module changes project usage or architecture.

## Success Condition
Competition CRUD is implemented, persisted in PostgreSQL, exposed through the API, and fully covered by unit/domain/integration tests with all tests passing locally.

## Known Issues / Decisions To Review
- Teams update endpoint currently uses `POST /teams/update` rather than conventional `PUT /teams/{id}`. This remains a future API consistency refactor, not part of the Competition implementation unless the existing conventions are intentionally changed.
- Competition-to-Team relationships will be introduced later when the football domain model requires them.

## Important Decisions
- Keep tests organized by business feature first, then test type.
- Keep API request/response DTOs in `FootballDataPlatform.Contracts`.
- Use Testcontainers PostgreSQL for integration tests.
- Keep `main` as the source of truth.
- Update `docs/PROJECT_STATE.md` after every meaningful project step so progress survives conversation/branch cleanup.

## Session Protocol
```bash
git status
git log -5 --oneline
cat docs/PROJECT_STATE.md
dotnet test
```

After each meaningful milestone, update:
- Current Milestone
- Completed / Previous Milestone
- Current Task
- Next Exact Steps
- Known Issues / Decisions
- Test verification result
