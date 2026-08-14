using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.Common;
using MediatR;

namespace FootballDataPlatform.Application.Teams;


public record DeleteTeamCommand(Guid Id) : IRequest<Result<Guid?>>;

internal class DeleteTeamHandler(ITeamRepository repository) : IRequestHandler<DeleteTeamCommand, Result<Guid?>>
{
    public async Task<Result<Guid?>> Handle(DeleteTeamCommand command, CancellationToken ct)
    {
        var team = await repository.GetByIdAsync(
            command.Id,
            ct);
        if (team is null)
            return Result<Guid?>.Failure("Team not found");
        await repository.DeleteAsync(team, ct);
        return Result<Guid?>.Success(team.Id);
    }
}

