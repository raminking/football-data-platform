# Football Data Platform

## Vision

Build a production-ready football data platform to demonstrate backend engineering, data engineering and system design skills.

## Goal

The primary goal of this project is to become a portfolio project for Backend/Data Engineer positions in Europe.

## Tech Stack

- ASP.NET Core 8
- PostgreSQL
- Entity Framework Core 8
- MediatR
- Vertical Slice Architecture
- Docker
- Swagger

## Architecture

The project follows a Vertical Slice Architecture with clear separation between:

- API
- Application
- Domain
- Infrastructure
- Contracts
- Tests

### Project Responsibilities

#### API

Responsible for:

- HTTP endpoints
- Request and response handling
- Mapping HTTP requests to application commands

#### Application

Responsible for:

- Use cases
- Commands and handlers
- Application contracts
- Repository abstractions
- Application-level business flow

The Application layer does not know about database implementation details.

#### Domain

Responsible for:

- Domain entities
- Domain rules
- Domain validation
- Core business concepts

The Domain layer does not depend on Infrastructure or database technologies.

#### Infrastructure

Responsible for:

- PostgreSQL
- Entity Framework Core
- Database context
- Entity configurations
- Repository implementations
- Database migrations
- Infrastructure dependency injection

#### Contracts

Contains contracts shared between API and other application boundaries where required.

#### Tests

Contains automated tests for the application and domain behavior.

---

## Create Team Flow

The current Team creation flow is:

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
PostgreSQL