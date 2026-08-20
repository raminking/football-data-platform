using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.Common;
using MediatR;

namespace FootballDataPlatform.Application.Match;

public record GetMatchQuery(Guid PublicId) : IRequest<Result<Domain.Match.Match>>;
internal sealed class GetMatchHandler(IMatchRepository repository) : IRequestHandler<GetMatchQuery, Result<Domain.Match.Match>>
{
    public async Task<Result<Domain.Match.Match>> Handle(GetMatchQuery query, CancellationToken ct)
    {
        var match = await repository.GetByPublicIdAsync(query.PublicId, ct);
        return match is null ? Result<Domain.Match.Match>.Failure("Match not found.") : Result<Domain.Match.Match>.Success(match);
    }
}