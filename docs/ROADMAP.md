# Roadmap

This document outlines the strategic development plan for the Football Data Platform.

---

## Sprint 1 — Foundation ✅
**Status:** Completed

- Solution structure
- Clean Architecture + Vertical Slice Architecture
- PostgreSQL & Entity Framework Core setup
- Dependency Injection configuration
- Initial Migration
- CI/CD Pipeline basics

---

## Sprint 2 — Teams Module ✅
**Status:** Completed

**Completed Features:**
- [x] Team Entity (Rich Domain Model)
- [x] Create Team (with uniqueness validation)
- [x] Get Team (by ID)
- [x] Update Team (with domain rules & duplicate checks)
- [x] Delete Team (Hard Delete)
- [x] Repository Pattern Implementation
- [x] Unit Tests (Domain & Application layers)
- [x] RESTful API Endpoints (Carter)

**Deferred / Future Improvements:**
- [ ] Optimistic Concurrency Control (RowVersion)
- [ ] Integration Tests (End-to-End)
- [ ] Soft Delete vs Hard Delete decision (ADR)

---

## Sprint 3 — Competitions Module 🚧
**Status:** Planned / Next

- Competition Entity (League, Cup)
- Season Entity
- Relationship: Competition has many Seasons
- Validation: Date ranges, unique names per season
- CRUD APIs

---

## Sprint 4 — Matches & Results
**Status:** Planned

- Match Entity
- MatchResult Value Object
- Scheduling logic
- Home/Away Team relationships
- Score tracking

---

## Sprint 5 — External Data Import
**Status:** Planned

- Football Data API Integration (e.g., API-Football)
- Background Jobs (Hangfire/Quartz)
- Data Synchronization Strategies
- Error Handling & Retries

---

## Sprint 6 — Production Readiness
**Status:** Planned

- Authentication (JWT) & Authorization
- Structured Logging (Serilog)
- Docker Containerization
- Health Checks
- Advanced CI/CD (Security Scans, Performance Tests)

