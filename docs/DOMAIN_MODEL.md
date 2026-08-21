# Football Data Platform — Domain Model

This document defines the current domain foundation and the **target extensibility direction** for the Football Analytics Platform.

The project is intentionally not trying to implement the entire football domain at once. The goal is to keep today's model small while ensuring future football-analysis capabilities can be added without redesigning the existing core.

## 1. Identifier Model

All persisted domain entities use two identifiers with different responsibilities:

```text
Internal Id  → long / PostgreSQL bigint → database relationships and joins
PublicId     → Guid / PostgreSQL uuid   → API-facing identifier
```

Internal database IDs are never exposed as the public API identifier. External provider IDs are stored separately through `ExternalIdentity` and never become domain primary keys.

## 2. Current Core Domain

The currently implemented foundation is:

```text
Team
Competition
  └── Season
        └── Match
             ├── HomeTeam → Team
             └── AwayTeam → Team
```

### Team

```text
Team
├── Id        (internal long)
├── PublicId  (public Guid)
├── Name
└── Country
```

### Competition

```text
Competition
├── Id        (internal long)
├── PublicId  (public Guid)
├── Name
├── Country
└── Code
```

A competition is independent of a particular season.

### Season

```text
Season
├── Id
├── PublicId
├── CompetitionId
├── Name
├── StartDate
└── EndDate
```

### Match — current v1

```text
Match
├── Id
├── PublicId
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

The current Match model intentionally remains small. Advanced match concepts are added as separate domain concepts rather than being packed into Match.

## 3. External Identity

External provider identifiers are not domain primary keys.

```text
ExternalIdentity
├── Id
├── Provider / SourceKey
├── EntityType
├── ExternalId
├── InternalEntityId
└── CreatedAtUtc
```

The database enforces uniqueness for:

```text
(Provider, EntityType, ExternalId)
```

This provides source-scoped mapping and supports idempotent imports from multiple authorized sources.

Provider-specific DTOs remain outside the Domain layer.

## 4. Strategic Domain Direction — Players

`Player` will be an independent domain entity. A player must not be owned by a Team because a player can represent multiple teams over a career.

```text
Player
├── Id
├── PublicId
├── Name / identity data
├── DateOfBirth?
└── RetiredAt?
```

### Retirement

Retirement is part of the Player lifecycle, not a deletion operation.

```text
RetiredAt = null       → no recorded retirement
RetiredAt = date       → player retired on that date
```

Historical team relationships and match records remain intact after retirement.

## 5. Player Positions

A player can have multiple normal positions. Position must not be treated as a single immutable property of Player.

Target relationship:

```text
Player
  └── PlayerPosition → Position
```

The player's normal positions describe their capabilities/profile. They do **not** determine where the player actually played in a particular match.

## 6. Player ↔ Team History

A player's relationship with a team is historical and must be represented separately from Player and Team.

Target concept:

```text
PlayerTeamAssignment
├── PlayerId
├── TeamId
├── StartDate
├── EndDate?
└── future contract/loan metadata as justified
```

This supports:

- multiple teams across a career;
- a player returning to a previous team;
- season or half-season participation;
- contract start/end dates;
- future loan support without redesigning Player.

A Team must not be deleted merely because a player is no longer assigned to it, and a Player must not be deleted because the player retired or left a team.

## 7. Match Team Context

A Match needs a team-specific context rather than placing every team-specific concept directly on Match.

Target concept:

```text
MatchTeam
├── MatchId
├── TeamId
├── Home/Away context
└── future team-specific match metadata
```

This gives a natural extension point for lineup, coach, formation and tactical information.

## 8. Match Lineup

Only players actually selected for the match squad need to be persisted in the basic lineup model.

```text
MatchLineup
├── MatchId
├── TeamId
├── PlayerId
├── Role          (Starter / Substitute)
├── PositionId?   (actual/announced match position when available)
├── JerseyNumber?
└── future lineup metadata as justified
```

The full Team roster must not be copied into every Match.

A player's normal position and their actual position in a match are separate concepts.

## 9. Predicted Lineup

Predicted lineup and actual lineup are different facts and must not be merged.

Target concept:

```text
PredictedMatchLineup
├── MatchId
├── TeamId
├── PlayerId
├── PredictedRole
├── PredictedPosition?
└── PredictedFormation?
```

If a source does not provide predicted lineups, no synthetic prediction data is created.

This separation enables future analysis such as prediction accuracy and predicted-vs-actual selection changes.

## 10. Player Availability for a Match

Information such as injury, suspension or coach decision is match-context data, not a replacement for the lineup.

Target concept:

```text
PlayerAvailability
├── MatchId
├── TeamId
├── PlayerId
├── Status
├── Reason?
├── ExpectedReturnDate?
└── Source metadata where justified
```

Initial status vocabulary can include:

```text
Injured
Suspended
Doubtful
Ill
Unavailable
CoachDecision
Other
```

Only known availability facts are stored. If no source reports an absence, no artificial record is created.

This structure supports future analysis such as team performance without a particular player, injury impact and selection decisions.

## 11. Coach

Coach is an independent concept because coaches can change over time.

Target relationship:

```text
Team
  └── TeamCoachAssignment
         ├── CoachId
         ├── StartDate
         └── EndDate?
```

For a Match, the coach in effect at that time can be resolved from the historical assignment. If a provider explicitly supplies match-specific coach information, a match-level reference can be added without changing the Team history model.

## 12. Formation and Tactical Model

Formation is not a permanent property of Team and is not a property of Player.

A team can use different formations in different matches and can change shape during a match.

Target concepts:

```text
Formation
├── Id
├── Code          (e.g. 4-3-3)
└── Name
```

Team-level tactical identity can be represented historically:

```text
TeamTacticalProfile
├── TeamId
├── CoachId?
├── FormationId?
├── PlayingStyle?
├── PressingStyle?
├── StartDate
└── EndDate?
```

This is different from the actual formation used in a Match.

For match analysis, the target model is:

```text
MatchTeamFormation
├── MatchId
├── TeamId
├── FormationId
├── Type          (Predicted / Starting / TacticalChange)
├── StartMinute
└── EndMinute?
```

This permits a match to evolve from, for example, `4-3-3` to `4-2-3-1` without changing the Team's historical profile.

## 13. Match Officials

Officials are independent people/entities and their role belongs to the Match context.

Target concept:

```text
MatchOfficial
├── MatchId
├── OfficialId
└── Role
```

Roles may include:

```text
Referee
AssistantReferee1
AssistantReferee2
FourthOfficial
VAR
AVAR
```

The role vocabulary can expand without redesigning Match or Official.

## 14. Match Events

Events are a first-class future domain concept because they are the basis for detailed football analytics.

Target concept:

```text
MatchEvent
├── MatchId
├── Minute
├── PlayerId?
├── TeamId?
├── EventType
└── event-specific details
```

Examples include:

```text
Goal
Assist
YellowCard
RedCard
Substitution
Penalty
OwnGoal
VAR
```

Event details should be extensible. We should not create a large fixed set of columns on Match for every possible event type.

Events and aggregated statistics are intentionally separate concepts.

## 15. Statistics Are Derived/Analytical Data

Detailed player/team statistics should not be confused with raw events.

Conceptually:

```text
Raw Match Data
      ↓
Match Events / Lineup / Context
      ↓
Match Statistics
      ↓
Season / Career Aggregations
```

Target analytical concepts may eventually include:

```text
PlayerMatchStatistics
TeamMatchStatistics
PlayerSeasonStatistics
TeamSeasonStatistics
```

These are introduced when actual data sources and use cases justify them. The core OLTP domain should not be overloaded with every possible analytical metric from day one.

## 16. Data Provenance

Because the platform is intended to support multiple sources, future rich football data must preserve source context where necessary.

The current `ExternalIdentity` mechanism is the foundation for identity mapping. Future source-specific facts may require additional provenance such as:

```text
SourceKey
ExternalId
RetrievedAtUtc
```

The Domain must remain independent from provider DTOs and private/unauthorized endpoints.

## 17. Delete and Historical Data Policy

Football data is historical and analytical. Deleting a Team, Player, Match or Competition can destroy information needed for future analysis.

Therefore:

- retirement is not deletion;
- leaving a team is not deletion;
- historical assignments remain queryable;
- referenced records must not be silently cascade-deleted;
- physical deletion should be restricted to cases where the domain explicitly permits it;
- soft delete/audit infrastructure is not added globally until a concrete lifecycle requirement justifies it.

When deletion behavior is implemented for a specific aggregate, foreign-key behavior and historical-data requirements must be reviewed together.

## 18. Design Principles

The following principles are now part of the project's domain direction:

1. **Small core, open extension points.** Do not implement speculative features, but do not put future concepts into the wrong existing entity.
2. **History is data.** Team membership, coaching, tactical profiles and retirement must preserve historical context.
3. **Match context is separate from master data.** A player's normal position is different from their match position; a team's default formation is different from its match formation.
4. **Predicted and actual facts are separate.** Never overwrite predictions with actual match data.
5. **Events are not statistics.** Raw events and derived/aggregated metrics have different responsibilities.
6. **Provider-neutral Domain.** External providers adapt into the platform; the Domain does not model a provider's response shape.
7. **No artificial data.** Missing provider data means missing data, not synthetic records.
8. **No destructive shortcuts.** Historical entities are not removed just because they are inactive.
9. **Avoid global generic blobs.** Structured football concepts should have structured domain models when they become real requirements.
10. **No mandatory big-bang implementation.** These concepts are extension targets, not a requirement to implement everything immediately.

## 19. Deferred Until Justified

The following remain intentionally deferred:

- full competition rules/format engine;
- transfer market and fee history;
- detailed contracts beyond the assignment model;
- complete injury/medical history;
- full event taxonomy;
- advanced tactical/positional tracking;
- xG and provider-specific advanced metrics;
- data warehouse / OLAP model;
- global audit/event-sourcing infrastructure;
- background scheduling before synchronous import behavior is production-ready.

The architecture must leave room for these features without requiring a rewrite of the existing core.
