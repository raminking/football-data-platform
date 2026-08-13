# Roadmap

## Sprint 1

### Project Initialization

- [x] Create Solution
- [x] Setup PostgreSQL
- [x] Docker Compose
- [x] Health Endpoint

### Team Creation Foundation

- [x] Configure Entity Framework Core
- [x] Create FootballDataDbContext
- [x] Create Team domain model
- [x] Create Team repository abstraction
- [x] Implement Team repository
- [x] Create TeamConfiguration
- [x] Create initial database migration
- [x] Apply database migration
- [x] Configure MediatR
- [x] Configure Application dependency injection
- [x] Configure Infrastructure dependency injection
- [x] Create Team command and handler
- [x] Create Team endpoint
- [x] Add duplicate Team + Country validation
- [x] Add Team domain tests
- [x] Test Team creation manually with curl
- [x] Move database credentials to ASP.NET Core User Secrets

### Sprint 1 Verification

- [x] `dotnet build`
- [x] `dotnet test`
- [x] Database migration successfully applied
- [x] Create Team API manually tested
- [x] Duplicate Team + Country rejected
- [x] Same Team name with different Country accepted

---

## Sprint 2

### Teams Module

- [ ] Review current Team creation implementation
- [ ] Improve API response design
- [ ] Add Get Team
- [ ] Add Get Teams
- [ ] Add Update Team
- [ ] Add Delete Team
- [ ] Add integration tests
- [ ] Add Swagger documentation
- [ ] Review persistence and domain design

### Players Module

- [ ] Create Player domain model
- [ ] Create Player repository abstraction
- [ ] Create Player persistence
- [ ] Create Player commands and queries
- [ ] Create Player endpoints
- [ ] Add Player tests

### Leagues Module

- [ ] Create League domain model
- [ ] Create League repository abstraction
- [ ] Create League persistence
- [ ] Create League commands and queries
- [ ] Create League endpoints
- [ ] Add League tests

---

## Sprint 3

### Football Data Integration

- [ ] Select external football data provider
- [ ] Define provider abstraction
- [ ] Implement external API integration
- [ ] Data synchronization
- [ ] Scheduler
- [ ] Background Jobs
- [ ] Retry handling
- [ ] Failure handling

---

## Sprint 4

### Statistics

- [ ] Match statistics
- [ ] Team statistics
- [ ] Player statistics
- [ ] Aggregation layer
- [ ] Statistics API
- [ ] Dashboard API

---

## Sprint 5

### Production Readiness

- [ ] Authentication
- [ ] Authorization
- [ ] Structured Logging
- [ ] Global Error Handling
- [ ] Health Checks
- [ ] Docker production setup
- [ ] CI/CD
- [ ] Automated database migrations
- [ ] Monitoring