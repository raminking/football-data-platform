using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.Common;
using MediatR;

namespace FootballDataPlatform.Application.Match;

public record DeleteMatchCommand(Guid Id) : IRequest<Result<bool>>;

internal sealed class DeleteMatchHandler(IMatchRepository repository)
    : IRequestHandler<DeleteMatchCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteMatchCommand command, CancellationToken ct)
    {
        var match = await repository.GetByIdAsync(command.Id, ct);
        if (match is null) return Result<bool>.Failure("Match not found.");

        await repository.DeleteAsync(match, ct);
        return Result<bool>.Success(true);
    }
}