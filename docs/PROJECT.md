# Football Data Platform

## Vision

Build a production-ready football data platform to demonstrate backend engineering, data engineering, database design, and system design skills.

## Goal

The primary goal of this project is to become a portfolio project for Backend Engineer and Data Engineer positions in Europe.

The project is being developed as a real-world backend system rather than as a collection of isolated tutorials.

---

## Tech Stack

- .NET 8
- ASP.NET Core 8
- PostgreSQL
- Entity Framework Core 8
- Npgsql
- MediatR
- Vertical Slice Architecture
- Docker
- Swagger
- xUnit

---

## Architecture

The project uses Vertical Slice Architecture.

The main projects are:

```text
FootballDataPlatform
│
├── src
│   ├── FootballDataPlatform.Api
│   ├── FootballDataPlatform.Application
│   ├── FootballDataPlatform.Contracts
│   ├── FootballDataPlatform.Domain
│   └── FootballDataPlatform.Infrastructure
│
├── tests
│   └── FootballDataPlatform.Tests
│
└── docs
    ├── ADR
    ├── LESSONS_LEARNED.md
    ├── PROJECT.md
    ├── ROADMAP.md
    └── SPRINTS.md