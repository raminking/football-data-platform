# ADR-004

## Title

Handle multiple external data sources without hiding data problems

---

## Status

Accepted

---

## Context

The project already uses a source resolver and the import services receive a source key explicitly. At the moment there is only one registered external source.

We will eventually have more than one source, but adding fallback now would make the import flow harder to reason about without having a real second provider to test against.

The main concern is that we should not switch providers for every kind of error. A provider being temporarily unavailable is different from returning invalid data or rejecting our credentials.

## Decision

For now, the requested source is used explicitly and there is no automatic fallback.

When a second provider is added, fallback will be handled by the import/source layer with an explicit priority order.

Fallback will be allowed for transient provider failures such as:

- timeout
- network failure
- rate limiting
- temporary server errors (5xx)

Fallback will not be used for errors that normally need configuration or code changes:

- authentication/authorization failures
- invalid responses
- invalid data
- mapping/processing errors

The first provider that returns a valid result wins. A lower-priority provider must not silently replace valid data from a higher-priority provider.

Provider priority will be configuration-driven rather than based on DI registration order.

## Consequences

For now the code stays simple because there is only one provider.

When another provider is introduced, the fallback rules are already defined and can be implemented without changing the domain model.

This also keeps provider failures visible instead of hiding real data or configuration problems behind an automatic fallback.
