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

A production-ready project foundation was established.

---

## Sprint 2: Teams Module (Completed)

**Goal:** Implement full CRUD operations for the Teams aggregate.

### Completed Stories
- [x] **Create Team:** As a user, I can register a new football team with name and country.
- [x] **Get Team:** As a user, I can retrieve details of a specific team by ID.
- [x] **Update Team:** As a user, I can update a team's details (enforcing uniqueness and domain rules).
- [x] **Delete Team:** As a user, I can remove a team from the system.

### Technical Implementation
- **Architecture:** Vertical Slice Architecture with Clean Architecture principles.
- **Pattern:** CQRS with MediatR.
- **Domain:** Rich Domain Model (encapsulated logic in `Team` entity).
- **Validation:** Business rules enforced in Domain layer; Uniqueness checked in Application layer.
- **API:** RESTful endpoints using Carter (`POST`, `GET`, `PUT`, `DELETE`).
- **Testing:**
    - Unit Tests for Domain logic (validations).
    - Unit Tests for Application Handlers (mocking repository).
    - *Pending: Integration Tests.*

### Key Decisions & Learnings
- Chose **Rich Domain Model** over Anemic to prevent invalid states.
- Implemented **Uniqueness Check** in Application layer to avoid database constraint exceptions.
- Used **Carter** for lightweight endpoint definition.
- *Lesson:* Handling naming conflicts in tests (Namespace vs Entity class) required using type aliases.

### Missing / Future Improvements
- [ ] **Concurrency Control:** Add RowVersion/Timestamp to handle concurrent updates (Optimistic Concurrency).
- [ ] **Integration Tests:** End-to-end tests using TestContainers or in-memory database.
- [ ] **Soft Delete:** Consider implementing soft delete instead of hard delete for audit trails (requires ADR).