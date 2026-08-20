using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.Common;
using FootballDataPlatform.Domain.Competitions;
using MediatR;

namespace FootballDataPlatform.Application.Competitions;

public record GetSeasonQuery(Guid PublicId) : IRequest<Result<Season>>;
internal sealed class GetSeasonHandler(ISeasonRepository repository) : IRequestHandler<GetSeasonQuery, Result<Season>>
{
    public async Task<Result<Season>> Handle(GetSeasonQuery query, CancellationToken ct)
    {
        var season = await repository.GetByPublicIdAsync(query.PublicId, ct);
        return season is null ? Result<Season>.Failure("Season not found.") : Result<Season>.Success(season);
    }
}