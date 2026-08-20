namespace FootballDataPlatform.Infrastructure.Persistence.ExternalData;

public sealed class ExternalIdentity
{
    private ExternalIdentity() { }

    public ExternalIdentity(string provider, string entityType, string externalId, long internalEntityId)
    {
        if (string.IsNullOrWhiteSpace(provider)) throw new ArgumentException("Provider is required.", nameof(provider));
        if (string.IsNullOrWhiteSpace(entityType)) throw new ArgumentException("Entity type is required.", nameof(entityType));
        if (string.IsNullOrWhiteSpace(externalId)) throw new ArgumentException("External ID is required.", nameof(externalId));
        if (internalEntityId <= 0) throw new ArgumentException("Internal entity ID is required.", nameof(internalEntityId));
        Provider = provider.Trim(); EntityType = entityType.Trim(); ExternalId = externalId.Trim();
        InternalEntityId = internalEntityId; CreatedAtUtc = DateTimeOffset.UtcNow;
    }

    public long Id { get; private set; }
    public string Provider { get; private set; } = null!;
    public string EntityType { get; private set; } = null!;
    public string ExternalId { get; private set; } = null!;
    public long InternalEntityId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
}