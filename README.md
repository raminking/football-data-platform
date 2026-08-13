# Football Data Platform

## Architecture

- Domain
- Application
- Infrastructure
- API
- Contracts
- Tests

## Current Features

- Create Team
- PostgreSQL persistence
- Entity Framework Core
- MediatR
- Domain validation
- Unique Team + Country constraint

## Database

PostgreSQL + Entity Framework Core.

Migrations are located in:

src/FootballDataPlatform.Infrastructure/Migrations/

## Local Development

Connection strings are stored using ASP.NET Core User Secrets.

Do not commit database passwords.

## Commands

dotnet build
dotnet test
dotnet ef database update

