# Football Data Platform

## Architecture

- **Domain** — football business entities and validation
- **Application** — use cases, import services and provider-neutral abstractions
- **Infrastructure** — PostgreSQL/EF Core persistence and external data providers
- **API** — Carter HTTP endpoints
- **Contracts** — API request/response contracts
- **Tests** — unit and integration tests

## External data / multi-source architecture

External football data is accessed through `IFootballDataSource`, resolved by `IFootballDataSourceResolver`. The application is therefore not coupled to `football-data.org` and additional providers can be added without changing the import use cases.

Current provider:

- `football-data.org` (`SourceKey = football-data.org`)

Current import pipeline:

```text
Competition → Season → Teams → Matches
```

`FootballDataImportOrchestrator` coordinates the four import services and aggregates their results.

## Import API

Run an end-to-end import with:

```text
POST /imports/{sourceKey}/{competitionCode}/{seasonYear}
```

Example:

```text
POST /imports/football-data.org/PL/2025
```

The response contains `Created`, `Updated`, `Skipped`, `Processed` and `Errors` totals.

Imports are designed to be idempotent. The first import creates missing records; repeating the same import resolves existing external identities and updates existing records instead of creating duplicates. For example, the current Premier League 2025/26 import has been verified with 541 matches created on the first run and 541 matches updated on the second run.

The provider receives `2025` as the football-data.org season parameter, while the persisted season name is `2025/26`. Do not query `Seasons.Year` for this value: the current season model stores the display value in `Seasons.Name`.

Check the current database row counts with:

```text
GET /imports/status
```

The status endpoint reports counts for competitions, seasons, teams, matches and external identities. It reads directly from PostgreSQL through EF Core and does not fabricate data. The local database is external runtime state and is therefore not stored in Git.

## Database initialization

The API initializes the configured PostgreSQL database by applying pending EF Core migrations at startup. This prevents the application from querying tables before the migration schema has been created, provided the configured database is reachable and migrations are present.

For manual local migration management:

```bash
dotnet ef database update
```

If PostgreSQL reports `42P01: relation "Competitions" does not exist`, check that the API is connected to the intended database and that migrations have been applied.

## Configuration

`football-data.org` requires its API token when the provider plan requires authentication. Configure it through User Secrets or environment-specific configuration; do not commit secrets.

The database connection uses the `DefaultConnection` connection string.

## Database

PostgreSQL + Entity Framework Core.

Migrations are located in:

`src/FootballDataPlatform.Infrastructure/Migrations/`

## Development verification

```bash
dotnet clean
dotnet build
dotnet test
```

The test suite currently passes with **102 tests passed, 0 failed, 0 skipped**. Keep the test suite green after import and persistence changes.

The repository intentionally uses a single `main` branch. No feature branches are required for this project workflow.

## Current verified state

The application currently contains competition, season, team and match import services, external identity mapping, provider-neutral external data abstractions, an end-to-end import orchestrator, PostgreSQL persistence, database migration initialization and API entry points for executing and inspecting imports.

A real local PostgreSQL database has been exercised through the import API. The verified Premier League 2025/26 import processed 541 matches: first run `541 created / 0 updated / 0 skipped`, repeated run `0 created / 541 updated / 0 skipped`. This confirms the current match import path is idempotent for that dataset.

The latest clean-up also removes a duplicate `using FootballDataPlatform.Domain.Competitions;` directive from the match import tests.

Actual database row counts can change independently of Git and should be checked with `/imports/status` or SQL against the configured local database when needed.
