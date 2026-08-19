using FootballDataPlatform.Application.Abstractions.ExternalData;

namespace FootballDataPlatform.Application.Abstractions.Persistence;

public interface IExternalIdentityRepository
{
    Task<ExternalIdentityRecord?> FindAsync(
        string provider,
        string entityType,
        string externalId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        ExternalIdentityRecord identity,
        CancellationToken cancellationToken = default);
}
