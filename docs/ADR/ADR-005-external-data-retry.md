# ADR-005

## Title

Retry transient external data failures

## Status

Accepted

## Context

External providers can fail temporarily because of rate limits, short outages, network problems or timeouts. Retrying every error would make the situation worse and could hide configuration or data problems.

## Decision

External HTTP requests use a small, bounded retry policy.

We retry:

- HTTP 429
- HTTP 5xx
- network failures
- request timeouts

We do not retry:

- HTTP 400
- HTTP 401
- HTTP 403
- invalid responses/data
- caller cancellation

The default policy allows two retries after the first attempt. Delays use exponential backoff with a small amount of jitter and are capped at 30 seconds.

For HTTP 429, the `Retry-After` header is respected when it is present, up to the configured maximum delay.

Retries happen in the HTTP handler, before the provider maps the final response to `ExternalDataException`. This keeps retry behavior out of the provider mapping code.

## Consequences

A temporary provider failure can recover without failing the whole import immediately.

The retry count is deliberately small so a broken provider does not create a long-running import or a retry storm.

The policy is configuration-driven, so the limits can be changed without changing provider code.
