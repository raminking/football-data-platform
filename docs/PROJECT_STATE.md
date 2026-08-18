# Football Data Platform — Project State

> Compact source of truth for continuing the project across sessions.

## Current Milestone
Competition module — migration/test failure being fixed.

## Previous Milestone — Teams
**Completed and locally verified:** `27 passed, 0 failed, 0 skipped`.

Teams CRUD, unit tests, domain tests, and PostgreSQL Testcontainers integration coverage are complete.

## Competition Module
Implemented on `main`:
- Competition domain entity with Name, Country and Code.
- Domain validation and code normalization.
- Create/Get/Update/Delete application operations.
- Competition repository abstraction and PostgreSQL implementation.
- EF Core configuration and unique identity constraint on Name + Country + Code.
- API contracts.
- Carter CRUD endpoints.
- EF Core migration and model snapshot.
- Competition domain tests.
- PostgreSQL API integration tests covering create, duplicate/validation, get, update, and delete.

### API
- `POST /competitions`
- `GET /competitions/{id}`
- `PUT /competitions/{id}`
- `DELETE /competitions/{id}`

## Latest Verification
Local `dotnet test` result:
- **33 passed**
- **8 failed**
- **0 skipped**
- **41 total**

### Failure Analysis
The primary failure is:
`42P01: relation "Competitions" does not exist`

The Competition migration file existed, but its generated EF Core migration designer metadata was missing. As a result, EF Core migration discovery did not correctly include/apply `AddCompetitions` to the Testcontainers database.

A migration designer file has now been added:
`20260818113000_AddCompetitions.Designer.cs`

The delete integration test also showed JSON parsing of a non-JSON error response; this is a downstream symptom of the missing `Competitions` table, not the root cause.

## Current Task
Re-run the complete test suite after the migration-designer fix and address any remaining failures.

## Next Exact Steps
1. Run `dotnet test` locally with Docker Desktop running.
2. If migration failures remain, inspect EF migration discovery/history and fix them.
3. If API behavior failures remain after the database is available, fix those independently.
4. Record the exact passing test count here.
5. If green, close Competition milestone.
6. Move immediately to Season and model Competition → Seasons.
7. Update all relevant docs after the Season milestone.

## Success Condition
Competition CRUD is implemented, persisted in PostgreSQL, exposed through the API, and covered by domain and integration tests with the complete suite passing locally.

## Known Issues / Decisions To Review
- Teams update endpoint remains `POST /teams/update`; Competition uses conventional `PUT /competitions/{id}`. API consistency can be refactored later.
- Competition-to-Team relationships are intentionally deferred.
- Competition code is currently part of uniqueness identity together with Name and Country.
- The Competition migration designer was missing and has been added; local verification is still required.

## Important Decisions
- Keep tests organized by business feature first, then test type.
- Keep API request/response DTOs in `FootballDataPlatform.Contracts`.
- Use Testcontainers PostgreSQL for integration tests.
- Keep `main` as the source of truth.
- Update `docs/PROJECT_STATE.md` after every meaningful project step.
- Keep strategic documentation synchronized across `PROJECT.md`, `ROADMAP.md`, `SPRINTS.md`, and `LESSONS_LEARNED.md` when milestones change.

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
