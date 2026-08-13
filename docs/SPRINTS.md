# Sprint History

## Sprint 1

### Day 1 — Project Initialization

- Repository created
- README added
- Initial architecture planning
- Solution structure created
- PostgreSQL selected
- Vertical Slice Architecture selected

Status:

✅ Completed

---

### Day 2 — Team Creation Foundation

#### Domain

- Team domain entity created
- Team validation implemented
- Team Id generated using Guid
- Team name trimming implemented
- Team country trimming implemented

#### Application

- MediatR configured
- CreateTeamCommand created
- CreateTeamHandler created
- Result abstraction created
- ITeamRepository abstraction created
- Application dependency injection configured

#### Infrastructure

- Entity Framework Core configured
- Npgsql configured
- FootballDataDbContext created
- TeamConfiguration created
- TeamRepository implemented
- Infrastructure dependency injection configured
- Unique Team + Country database constraint created
- Initial migration created

#### Database

- PostgreSQL database created
- Initial migration applied successfully
- Teams table created
- Unique Team + Country index created

#### API

- CreateTeamEndpoint created
- POST `/teams` implemented
- MediatR connected to the endpoint
- Duplicate Team + Country validation exposed through the API

#### Tests

- Team creation test added
- Empty Team name validation test added
- Empty Team country validation test added
- `dotnet test` completed successfully
- 3 tests passed

#### Manual API Verification

The following scenarios were tested:

```text
Arsenal + England
→ Successfully created

Arsenal + England
→ Rejected because it already exists

Arsenal + Spain
→ Successfully created