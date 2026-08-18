using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.Common;
using FootballDataPlatform.Domain.Match;
using MediatR;

namespace FootballDataPlatform.Application.Match;

public record GetMatchQuery(Guid Id) : IRequest<Result<Domain.Match.Match>>;

internal sealed class GetMatchHandler(IMatchRepository repository)
    : IRequestHandler<GetMatchQuery, Result<Domain.Match.Match>>
{
    public async Task<Result<Domain.Match.Match>> Handle(GetMatchQuery query, CancellationToken ct)
    {
        var match = await repository.GetByIdAsync(query.Id, ct);
        return match is null
            ? Result<Domain.Match.Match>.Failure("Match not found.")
            : Result<Domain.Match.Match>.Success(match);
    }
}