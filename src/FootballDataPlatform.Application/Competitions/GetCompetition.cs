using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Domain.Competitions;
using MediatR;

namespace FootballDataPlatform.Application.Competitions;

public record GetCompetitionQuery(Guid PublicId) : IRequest<Competition?>;
internal sealed class GetCompetitionHandler(ICompetitionRepository repository) : IRequestHandler<GetCompetitionQuery, Competition?>
{
    public Task<Competition?> Handle(GetCompetitionQuery query, CancellationToken ct) => repository.GetByPublicIdAsync(query.PublicId, ct);
}