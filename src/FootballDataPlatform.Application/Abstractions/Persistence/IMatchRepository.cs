using FootballDataPlatform.Domain.Match;

namespace FootballDataPlatform.Application.Abstractions.Persistence;

public interface IMatchRepository
{
    Task<bool> SeasonExistsAsync(Guid seasonId, CancellationToken cancellationToken);
    Task<bool> TeamExistsAsync(Guid teamId, CancellationToken cancellationToken);
    Task CreateAsync(Match match, CancellationToken cancellationToken);
    Task<Match?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateAsync(Match match, CancellationToken cancellationToken);
    Task DeleteAsync(Match match, CancellationToken cancellationToken);
}