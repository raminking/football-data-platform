using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.Common;
using FootballDataPlatform.Domain.Teams;
using MediatR;

namespace FootballDataPlatform.Application.Teams.CreateTeam;


public record CreateTeamCommand(string Name, string Country) : IRequest<Result<Guid>>;

internal class CreateTeamHandler(ITeamRepository repository) : IRequestHandler<CreateTeamCommand, Result<Guid>>
{
public async Task<Result<Guid>> Handle(CreateTeamCommand command, CancellationToken ct)
{
    if (string.IsNullOrWhiteSpace(command.Name))
    {
        return Result<Guid>.Failure("Team name is required.");

    }

    if (string.IsNullOrWhiteSpace(command.Country))
    {
        return
            Result<Guid>.Failure("Team country is required.");
        ;
    }
    if(await repository.ExistsByNameAsync(
        command.Name,
        command.Country,
        ct))
    {
        return Result<Guid>.Failure("exists in database");

    }
    var team = new Team(
        command.Name,
        command.Country);
    await repository.CreateAsync(team, ct);
 
    return Result<Guid>.Success(team.Id);
}
}

