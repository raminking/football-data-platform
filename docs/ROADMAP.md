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

## Sprint 3 — Competitions & Seasons ✅
**Status:** Completed and locally verified

### Competition
- [x] Competition Entity (League/Cup)
- [x] Create/Get/Update/Delete
- [x] Validation and uniqueness rules
- [x] PostgreSQL persistence
- [x] Carter API
- [x] Domain and integration tests
- [x] EF Core migration and designer metadata

### Season
- [x] Season Entity
- [x] Competition → Seasons relationship
- [x] Date-range validation
- [x] Unique season name within competition
- [x] Create/Get/Update/Delete
- [x] PostgreSQL persistence and EF migration
- [x] Carter API
- [x] Integration coverage
- [x] Full suite verified: **51 passed, 0 failed, 0 skipped**

---

## Sprint 4 — Matches & Results 🚧
**Status:** In Progress

### Initial Scope
- Match Entity
- Competition Season relationship
- Home/Away Team relationships
- Kickoff date/time
- Match Status
- MatchResult / Score model
- Scheduling and domain validation
- CRUD APIs
- Unit/domain and PostgreSQL integration tests

### Current Order
1. Match domain model
2. Match result/status model
3. Application operations
4. PostgreSQL persistence
5. API endpoints
6. Integration test coverage

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
