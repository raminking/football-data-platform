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

Check the current database row counts with:

```text
GET /imports/status
```

The status endpoint reports counts for competitions, seasons, teams, matches and external identities. It does not fabricate data: counts come directly from PostgreSQL through EF Core.

## Configuration

`football-data.org` requires its API token when the provider plan requires authentication. Configure it through User Secrets or environment-specific configuration; do not commit secrets.

The database connection uses the `DefaultConnection` connection string.

## Database

PostgreSQL + Entity Framework Core.

Migrations are located in:

`src/FootballDataPlatform.Infrastructure/Migrations/`

Apply migrations locally with:

```bash
dotnet ef database update
```

## Development verification

```bash
dotnet clean
dotnet build
dotnet test
```

The repository intentionally uses a single `main` branch. No feature branches are required for this project workflow.

## Current status

The application layer contains competition, season, team and match import services, external identity mapping, a multi-source provider abstraction, an end-to-end import orchestrator and API entry points for executing and inspecting imports.

Actual PostgreSQL data must be verified by running `/imports/status` against the configured local database. The source repository itself cannot claim a local database row count because the database is external to Git.
