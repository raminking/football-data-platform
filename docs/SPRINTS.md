# Sprint History

---

# Sprint 1 — Project Foundation

Status

✅ Completed

Completed

- Created solution
- Established project structure
- Adopted Vertical Slice Architecture
- Configured Dependency Injection
- Configured PostgreSQL
- Configured Entity Framework Core
- Created initial migration

Outcome

A production-oriented project foundation was established.

---

## Sprint 2: Teams Module

**Status: ✅ Completed and locally verified**

**Goal:** Implement full CRUD operations for the Teams aggregate.

### Completed Stories
- [x] **Create Team:** Register a new football team with name and country.
- [x] **Get Team:** Retrieve details of a specific team by ID.
- [x] **Update Team:** Update team details with uniqueness and domain rules.
- [x] **Delete Team:** Remove a team from the system.

### Technical Implementation
- Vertical Slice Architecture with Clean Architecture principles.
- CQRS with MediatR.
- Rich Domain Model.
- Repository abstraction in Application and implementation in Infrastructure.
- Carter API endpoints.
- PostgreSQL persistence with EF Core.
- Testcontainers PostgreSQL integration tests.

### Verification
- **27 passed, 0 failed, 0 skipped**

### Deferred / Future Improvements
- [ ] Optimistic Concurrency Control.
- [ ] Soft Delete vs Hard Delete ADR.
- [ ] Review update endpoint route consistency.

---

## Sprint 3: Competitions & Seasons

**Status: ✅ Completed and locally verified**

**Goal:** Build the competition and season domain required for the football platform.

### Completed Stories
- [x] **Competition Entity:** Support league and cup competitions.
- [x] **Create/Get/Update/Delete Competition:** Validated competition identity and lifecycle operations.
- [x] **Season Entity:** Model a specific competition edition.
- [x] **Competition → Season relationship:** Establish the relationship with validation.
- [x] **Season date-range validation.**
- [x] **Unique season name within competition.**
- [x] **PostgreSQL persistence and EF migration.**
- [x] **Carter API.**
- [x] **Domain/Application/integration coverage.**

### Verification
- **51 passed, 0 failed, 0 skipped**

---

## Sprint 4: Matches & Results

**Status: 🚧 In Progress — Domain Design Completed**

**Goal:** Implement the core Match domain that connects Teams to a specific Competition Season.

### Domain Design Completed
- [x] Match v1 entity shape defined.
- [x] Season → Match relationship defined.
- [x] HomeTeam and AwayTeam relationships defined.
- [x] Match status lifecycle defined.
- [x] Match stage kept intentionally simple.
- [x] Full-time and half-time score model defined.
- [x] Result semantics defined.
- [x] Result/score consistency identified as a domain invariant.
- [x] Intentional MVP boundaries documented.

### Match v1

```text
Match
├── Id
├── SeasonId
├── HomeTeamId
├── AwayTeamId
├── ScheduledAt
├── Stage
├── Status
├── HomeScore
├── AwayScore
├── HalfTimeHomeScore
├── HalfTimeAwayScore
└── Result
```

### Next Implementation Stories
- [ ] Match domain implementation.
- [ ] Domain invariants and validation tests.
- [ ] Application CRUD operations.
- [ ] Repository abstraction and PostgreSQL implementation.
- [ ] EF configuration and migration.
- [ ] Carter API endpoints.
- [ ] PostgreSQL/Testcontainers integration coverage.
- [ ] Full suite verification.
- [ ] Documentation synchronization after implementation.

### Deliberately Deferred
- [ ] Extra-time and penalty-shootout scores.
- [ ] Goals, cards, substitutions and detailed match events.
- [ ] Lineups, referee, venue and weather.
- [ ] Competition formats, groups and qualification rules.
- [ ] Season participants and promotion/relegation rules.

---

## Sprint 5: External Data Import

**Status: Planned**

---

## Sprint 6: Production Readiness

**Status: Planned**
