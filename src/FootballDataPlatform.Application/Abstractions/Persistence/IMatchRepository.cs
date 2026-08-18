using MatchEntity = FootballDataPlatform.Domain.Match.Match;

namespace FootballDataPlatform.Application.Abstractions.Persistence;

public interface IMatchRepository
{
    Task<bool> SeasonExistsAsync(Guid seasonId, CancellationToken cancellationToken);
    Task<bool> TeamExistsAsync(Guid teamId, CancellationToken cancellationToken);
    Task CreateAsync(MatchEntity match, CancellationToken cancellationToken);
    Task<MatchEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateAsync(MatchEntity match, CancellationToken cancellationToken);
    Task DeleteAsync(MatchEntity match, CancellationToken cancellationToken);
}