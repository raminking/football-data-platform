using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Domain.Teams;
using MediatR;

namespace FootballDataPlatform.Application.Teams.GetTeam;

public record GetTeamQuery(Guid Id) : IRequest<Team?>;

internal class GetTeamHandler(ITeamRepository repository) : IRequestHandler<GetTeamQuery, Team?>
{
    public async Task<Team?> Handle(GetTeamQuery query, CancellationToken ct)
    {
        return await repository.GetByIdAsync(query.Id, cancellationToken: ct);
    }
}