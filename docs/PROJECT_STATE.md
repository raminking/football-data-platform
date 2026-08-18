# Football Data Platform — Project State

> Compact source of truth for continuing the project across sessions.

## Current Milestone
Competition module — CRUD implementation and test verification.

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
- EF Core migration and updated model snapshot.
- Competition domain tests.
- PostgreSQL API integration tests covering create, duplicate/validation, get, update, and delete.

### API
- `POST /competitions`
- `GET /competitions/{id}`
- `PUT /competitions/{id}`
- `DELETE /competitions/{id}`

## Current Task
Run and verify the complete test suite locally. The GitHub connector cannot execute `dotnet test`, so local verification is required before declaring the Competition milestone complete.

## Next Exact Steps
1. Run `dotnet test` locally with Docker Desktop running.
2. Fix any compilation/test/migration issues found.
3. Record the exact passing test count here.
4. If green, close Competition milestone.
5. Move immediately to the Season module and model Competition → Seasons.
6. Update all relevant docs after the Season milestone as done for Teams and Competition.

## Success Condition
Competition CRUD is implemented, persisted in PostgreSQL, exposed through the API, and covered by domain and integration tests with the complete suite passing locally.

## Known Issues / Decisions To Review
- Teams update endpoint remains `POST /teams/update`; Competition uses conventional `PUT /competitions/{id}`. API consistency can be refactored later.
- Competition-to-Team relationships are intentionally deferred.
- Competition code is currently part of uniqueness identity together with Name and Country.
- Migration and model snapshot were created through the repository workflow but should be validated by `dotnet test`/EF migration execution locally.

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
