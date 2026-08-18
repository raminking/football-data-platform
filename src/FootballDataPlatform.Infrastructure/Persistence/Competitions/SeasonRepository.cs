using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Domain.Competitions;
using Microsoft.EntityFrameworkCore;

namespace FootballDataPlatform.Infrastructure.Persistence.Competitions;

public class SeasonRepository(FootballDataDbContext db) : ISeasonRepository
{
    public Task<bool> CompetitionExistsAsync(Guid competitionId, CancellationToken cancellationToken) =>
        db.Competitions.AnyAsync(x => x.Id == competitionId, cancellationToken);

    public Task<bool> ExistsByIdentityAsync(Guid competitionId, string name, CancellationToken cancellationToken) =>
        db.Seasons.AnyAsync(x => x.CompetitionId == competitionId && x.Name == name, cancellationToken);

    public async Task CreateAsync(Season season, CancellationToken cancellationToken)
    {
        await db.Seasons.AddAsync(season, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    public Task<Season?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.Seasons.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task UpdateAsync(Season season, CancellationToken cancellationToken)
    {
        db.Seasons.Update(season);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Season season, CancellationToken cancellationToken)
    {
        db.Seasons.Remove(season);
        await db.SaveChangesAsync(cancellationToken);
    }
}