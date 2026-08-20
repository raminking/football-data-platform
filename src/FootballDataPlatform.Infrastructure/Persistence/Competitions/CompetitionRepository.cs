using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Domain.Competitions;
using Microsoft.EntityFrameworkCore;

namespace FootballDataPlatform.Infrastructure.Persistence.Competitions;

public class CompetitionRepository(FootballDataDbContext context) : ICompetitionRepository
{
    public Task<bool> ExistsByIdentityAsync(string name, string country, string code, long? excludeId, CancellationToken cancellationToken) =>
        context.Competitions.AnyAsync(x => x.Name == name && x.Country == country && x.Code == code && (!excludeId.HasValue || x.Id != excludeId.Value), cancellationToken);
    public async Task CreateAsync(Competition competition, CancellationToken cancellationToken) { context.Competitions.Add(competition); await context.SaveChangesAsync(cancellationToken); }
    public Task<Competition?> GetByIdAsync(long id, CancellationToken cancellationToken) => context.Competitions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    public Task<Competition?> GetByPublicIdAsync(Guid publicId, CancellationToken cancellationToken) => context.Competitions.FirstOrDefaultAsync(x => x.PublicId == publicId, cancellationToken);
    public async Task UpdateAsync(Competition competition, CancellationToken cancellationToken) { context.Competitions.Update(competition); await context.SaveChangesAsync(cancellationToken); }
    public async Task DeleteAsync(Competition competition, CancellationToken cancellationToken) { context.Competitions.Remove(competition); await context.SaveChangesAsync(cancellationToken); }
}