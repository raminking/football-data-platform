using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Domain.Match;
using Microsoft.EntityFrameworkCore;

namespace FootballDataPlatform.Infrastructure.Persistence.Match;

public sealed class MatchRepository(FootballDataDbContext db) : IMatchRepository
{
    public Task<long?> GetSeasonIdByPublicIdAsync(Guid publicId, CancellationToken cancellationToken) => db.Seasons.Where(x => x.PublicId == publicId).Select(x => (long?)x.Id).SingleOrDefaultAsync(cancellationToken);
    public Task<long?> GetTeamIdByPublicIdAsync(Guid publicId, CancellationToken cancellationToken) => db.Teams.Where(x => x.PublicId == publicId).Select(x => (long?)x.Id).SingleOrDefaultAsync(cancellationToken);
    public async Task CreateAsync(Domain.Match.Match match, CancellationToken cancellationToken) { await db.Matches.AddAsync(match, cancellationToken); await db.SaveChangesAsync(cancellationToken); }
    public Task<Domain.Match.Match?> GetByIdAsync(long id, CancellationToken cancellationToken) => db.Matches.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    public Task<Domain.Match.Match?> GetByPublicIdAsync(Guid publicId, CancellationToken cancellationToken) => db.Matches.SingleOrDefaultAsync(x => x.PublicId == publicId, cancellationToken);
    public async Task UpdateAsync(Domain.Match.Match match, CancellationToken cancellationToken) { db.Matches.Update(match); await db.SaveChangesAsync(cancellationToken); }
    public async Task DeleteAsync(Domain.Match.Match match, CancellationToken cancellationToken) { db.Matches.Remove(match); await db.SaveChangesAsync(cancellationToken); }
}