using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Domain.Teams;
using MediatR;

namespace FootballDataPlatform.Application.Teams;

public record GetTeamQuery(Guid PublicId) : IRequest<Team?>;

internal class GetTeamHandler(ITeamRepository repository) : IRequestHandler<GetTeamQuery, Team?>
{
    public Task<Team?> Handle(GetTeamQuery query, CancellationToken ct) => repository.GetByPublicIdAsync(query.PublicId, ct);
}