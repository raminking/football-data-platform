# Lessons Learned

---

## Architecture

### Vertical Slice Architecture

Instead of organizing code by technical layers, features are organized together.

Example

Teams

- CreateTeam
- GetTeam

Each feature contains everything required for that use case.

Benefits

- Better scalability
- Better maintainability
- Easier feature development
- Reduced coupling

---

### Repository Pattern

The Application layer depends on abstractions rather than Entity Framework.

Example

Application

ITeamRepository

Infrastructure

TeamRepository

Benefits

- Loose coupling
- Easier testing
- Infrastructure can change without affecting business logic

---

## Entity Framework Core

Learned

- DbContext configuration
- Entity Configuration
- Fluent API
- Migrations
- Database Update

---

## PostgreSQL

Selected because

- Excellent performance
- Open Source
- Widely used in Europe
- Excellent EF Core support

---

## Dependency Injection

Application registers MediatR.

Infrastructure registers repositories and DbContext.

API composes the application.

---

## Minimal APIs

Minimal APIs provide a lightweight way to expose endpoints.

Benefits

- Less boilerplate
- Better readability
- Works well with Vertical Slice Architecture

---

## Testing

Current strategy

- Domain Tests
- Application Tests

Future

- Integration Tests
- API Tests