# ADR-003

## Title

Implement Teams feature using Vertical Slice Architecture

---

## Status

Accepted

---

## Context

The Teams module is the first complete business feature of the project.

It serves as the reference implementation for future modules.

---

## Decision

Each feature is implemented as an independent Vertical Slice.

Example

Teams

- CreateTeam
- GetTeam

Each slice contains

- Request
- Handler
- Endpoint

---

## Consequences

Advantages

- Feature isolation
- Easier maintenance
- Better scalability
- Cleaner architecture

Disadvantages

- More folders
- More files

The advantages outweigh the disadvantages for medium and large projects.