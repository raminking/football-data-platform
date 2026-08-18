using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Domain.Match;
using Microsoft.EntityFrameworkCore;

namespace FootballDataPlatform.Infrastructure.Persistence.Match;

public sealed class MatchRepository(FootballDataDbContext db) : IMatchRepository
{
    public Task<bool> SeasonExistsAsync(Guid seasonId, CancellationToken cancellationToken) =>
        db.Seasons.AnyAsync(x => x.Id == seasonId, cancellationToken);

    public Task<bool> TeamExistsAsync(Guid teamId, CancellationToken cancellationToken) =>
        db.Teams.AnyAsync(x => x.Id == teamId, cancellationToken);

    public async Task CreateAsync(Domain.Match.Match match, CancellationToken cancellationToken)
    {
        await db.Matches.AddAsync(match, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    public Task<Domain.Match.Match?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.Matches.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task UpdateAsync(Domain.Match.Match match, CancellationToken cancellationToken)
    {
        db.Matches.Update(match);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Domain.Match.Match match, CancellationToken cancellationToken)
    {
        db.Matches.Remove(match);
        await db.SaveChangesAsync(cancellationToken);
    }
}