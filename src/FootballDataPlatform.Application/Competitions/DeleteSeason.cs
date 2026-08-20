using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.Common;
using MediatR;

namespace FootballDataPlatform.Application.Competitions;

public record DeleteSeasonCommand(Guid PublicId) : IRequest<Result<bool>>;
internal sealed class DeleteSeasonHandler(ISeasonRepository repository) : IRequestHandler<DeleteSeasonCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteSeasonCommand command, CancellationToken ct)
    {
        var season = await repository.GetByPublicIdAsync(command.PublicId, ct);
        if (season is null) return Result<bool>.Failure("Season not found.");
        await repository.DeleteAsync(season, ct);
        return Result<bool>.Success(true);
    }
}