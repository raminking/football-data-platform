# Football Data Platform — Current Domain Model

This document defines the current MVP domain model and its intentional boundaries.

## 1. Team

A `Team` represents a football club or national team.

```text
Team
├── Id
├── Name
├── ShortName
├── Code
└── CountryId
```

The domain deliberately does not distinguish Club vs National Team yet. That distinction can be introduced later if required.

## 2. Competition

A `Competition` represents a competition independently of a particular season or edition.

```text
Competition
├── Id
├── Name
├── Country
└── Code
```

Examples include Premier League, FA Cup, Champions League, Europa League, World Cup and Friendly.

A competition does not represent a particular year.

## 3. Season

`Season` represents a specific edition of a competition.

```text
Season
├── Id
├── CompetitionId
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

## 4. Match — v1

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

### Relationships

```text
Competition
└── Season
    └── Match
        ├── HomeTeam
        └── AwayTeam
```

A Match belongs to a Season, not directly to Competition. The Season identifies the competition edition in which the match takes place.

A Team can therefore participate in matches across multiple seasons and competitions.

## 5. Match Stage

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

## 6. Match Status

The Match lifecycle is:

```text
Scheduled
InProgress
Finished
Postponed
Cancelled
Abandoned
```

This distinguishes scheduled fixtures from postponed, cancelled, abandoned and completed matches.

## 7. Scores

The MVP stores:

```text
HomeScore
AwayScore
HalfTimeHomeScore
HalfTimeAwayScore
```

Scores are nullable until meaningful match results exist.

The following are intentionally deferred:

- Extra-time score
- Penalty-shootout score
- Goals/events
- Cards
- Substitutions
- Possession
- Shots
- Corners
- Lineups
- Referee
- Venue
- Weather

## 8. Result

Result represents the match outcome:

```text
HomeWin
Draw
AwayWin
```

Result must be consistent with the final scores.

Examples:

```text
3 - 1 → HomeWin
1 - 1 → Draw
0 - 2 → AwayWin
```

An inconsistent state such as `3 - 1 + Draw` must not be accepted by the domain/application model.

Where practical, Result should be derived from final scores rather than treated as an independently mutable value.

## 9. Intentionally Deferred Domain Areas

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

## 10. MVP Boundary

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
