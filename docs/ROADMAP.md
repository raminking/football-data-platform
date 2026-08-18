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
**Status:** Completed and locally verified

**Completed Features:**
- [x] Team Entity (Rich Domain Model)
- [x] Create Team (with uniqueness validation)
- [x] Get Team (by ID)
- [x] Update Team (with domain rules & duplicate checks)
- [x] Delete Team (Hard Delete)
- [x] Repository Pattern Implementation
- [x] Unit Tests (Domain & Application layers)
- [x] API Endpoints (Carter)
- [x] PostgreSQL integration tests with Testcontainers
- [x] Full test suite verified: 27 passed, 0 failed, 0 skipped

**Deferred / Future Improvements:**
- [ ] Optimistic Concurrency Control (RowVersion)
- [ ] Soft Delete vs Hard Delete decision (ADR)
- [ ] Review/update endpoint route consistency (`POST /teams/update` vs conventional `PUT /teams/{id}`)

---

## Sprint 3 — Competitions Module 🚧
**Status:** In Progress

### Initial Scope
- Competition Entity (League, Cup)
- Season Entity
- Relationship: Competition has many Seasons
- Validation: date ranges and competition/season uniqueness rules
- CRUD APIs
- Unit, domain, and PostgreSQL integration tests

### Current Order
1. Competition domain model
2. Competition CRUD
3. Season domain model
4. Competition → Seasons relationship
5. Integration/API coverage

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
