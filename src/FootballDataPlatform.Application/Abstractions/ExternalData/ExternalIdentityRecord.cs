namespace FootballDataPlatform.Application.Abstractions.ExternalData;

public sealed record ExternalIdentityRecord(
    string Provider,
    string EntityType,
    string ExternalId,
    long InternalEntityId,
    DateTimeOffset CreatedAtUtc);
