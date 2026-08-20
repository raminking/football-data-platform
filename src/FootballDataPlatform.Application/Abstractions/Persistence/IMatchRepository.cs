using MatchEntity = FootballDataPlatform.Domain.Match.Match;

namespace FootballDataPlatform.Application.Abstractions.Persistence;

public interface IMatchRepository
{
    Task<long?> GetSeasonIdByPublicIdAsync(Guid publicId, CancellationToken cancellationToken);
    Task<long?> GetTeamIdByPublicIdAsync(Guid publicId, CancellationToken cancellationToken);
    Task CreateAsync(MatchEntity match, CancellationToken cancellationToken);
    Task<MatchEntity?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<MatchEntity?> GetByPublicIdAsync(Guid publicId, CancellationToken cancellationToken);
    Task UpdateAsync(MatchEntity match, CancellationToken cancellationToken);
    Task DeleteAsync(MatchEntity match, CancellationToken cancellationToken);
}