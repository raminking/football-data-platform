using FootballDataPlatform.Domain.Competitions;

namespace FootballDataPlatform.Application.Abstractions.Persistence;

public interface ICompetitionRepository
{
    Task<bool> ExistsByIdentityAsync(string name, string country, string code, long? excludeId, CancellationToken cancellationToken);
    Task CreateAsync(Competition competition, CancellationToken cancellationToken);
    Task<Competition?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<Competition?> GetByPublicIdAsync(Guid publicId, CancellationToken cancellationToken);
    Task UpdateAsync(Competition competition, CancellationToken cancellationToken);
    Task DeleteAsync(Competition competition, CancellationToken cancellationToken);
}