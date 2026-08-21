# Football Data Platform — Current Domain Model

This document defines the current MVP domain model and its intentional boundaries.

## 1. Identifier Model

All persisted domain entities use two identifiers with different responsibilities:

```text
Internal Id  → long / PostgreSQL bigint → database relationships and joins
PublicId     → Guid / PostgreSQL uuid   → API-facing identifier
```

Internal database IDs are never exposed as the public API identifier. External provider IDs are stored separately through `ExternalIdentity` and never become domain primary keys.

## 2. Team

A `Team` represents a football club or national team.

```text
Team
├── Id        (internal long)
├── PublicId  (public Guid)
├── Name
└── Country
```

We deliberately do not distinguish Club vs National Team yet. We also do not introduce `ShortName`, `Code`, or a separate `Country` entity until the domain requires them.

## 3. Competition

A `Competition` represents a competition independently of a particular season or edition.

```text
Competition
├── Id        (internal long)
├── PublicId  (public Guid)
├── Name
├── Country
└── Code
```

Examples include Premier League, FA Cup, Champions League, Europa League, World Cup and Friendly.

A competition does not represent a particular year.

## 4. Season

`Season` represents a specific edition of a competition.

```text
Season
├── Id           (internal long)
├── PublicId     (public Guid)
├── CompetitionId (internal long FK)
├── Name
├── StartDate
└── EndDate
```

Examples:

- Premier League 2025/26
- Premier League 2026/27
- World Cup 2026
- Champions League 2025/26

The number of participating teams belongs to the specific Season rather than Competition because competition sizes can change between editions.

## 5. Match — v1

```text
Match
├── Id               (internal long)
├── PublicId         (public Guid)
├── SeasonId         (internal long FK)
├── HomeTeamId       (internal long FK)
├── AwayTeamId       (internal long FK)
├── ScheduledAt
├── Stage
├── Status
├── HomeScore
├── AwayScore
├── HalfTimeHomeScore
├── HalfTimeAwayScore
└── Result
```

### Relationships

```text
Competition
└── Season
    └── Match
        ├── HomeTeam → Team
        └── AwayTeam → Team
```

A Match belongs to a Season, not directly to Competition. The Season identifies the competition edition in which the match takes place.

A Team can participate in matches across multiple seasons and competitions.

## 6. Match Stage

Stage is intentionally simple in v1. Supported concepts include:

- League
- Group Stage
- League Phase
- Playoff
- Round of 16
- Quarter Final
- Semi Final
- Final
- Friendly

A dedicated competition-format/rules subsystem is deliberately deferred.

## 7. Match Status

The Match lifecycle is:

```text
Scheduled
InProgress
Finished
Postponed
Cancelled
Abandoned
```

## 8. Scores

The MVP stores:

```text
HomeScore
AwayScore
HalfTimeHomeScore
HalfTimeAwayScore
```

Scores are nullable until meaningful match results exist.

Extra-time, penalty-shootout score, goals/events, cards, substitutions, possession, shots, corners, lineups, referee, venue and weather remain intentionally deferred.

## 9. Result

Result represents the match outcome:

```text
HomeWin
Draw
AwayWin
```

Result must be consistent with the final scores. For example:

```text
3 - 1 → HomeWin
1 - 1 → Draw
0 - 2 → AwayWin
```

Where practical, Result should be derived from final scores rather than treated as an independently mutable value.

## 10. External Identity

External provider identifiers are not domain primary keys.

```text
ExternalIdentity
├── Id               (internal long)
├── Provider/SourceKey
├── EntityType
├── ExternalId
├── InternalEntityId (internal long)
└── CreatedAtUtc
```

The database enforces uniqueness for:

```text
(Provider, EntityType, ExternalId)
```

This provides source-scoped mapping and supports idempotent imports from multiple authorized sources.

## 11. Intentionally Deferred Domain Areas

The MVP does not model:

- Competition formats
- Groups
- Season participants
- Promotion/relegation rules
- Points systems
- Head-to-head rules
- Goal-difference rules
- Qualification rules
- Detailed match events

These can be introduced later without changing the fundamental `Competition → Season → Match` relationship.

## 12. MVP Boundary

```text
Team
  ↓
Competition
  ↓
Season
  ↓
Match
```

The objective is to establish a small, coherent and production-oriented football data foundation before adding advanced competition and match-event concepts.