using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.Common;
using MediatR;

namespace FootballDataPlatform.Application.Teams;

public record UpdateTeamCommand(
    Guid Id,
    string Name,
    string Country,
    string? LogoUrl = null,
    string? OfficialWebsiteUrl = null) : IRequest<Result<Guid>>;

internal class UpdateTeamHandler(ITeamRepository repository) : IRequestHandler<UpdateTeamCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(UpdateTeamCommand command, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            return Result<Guid>.Failure("Team name is required.");

        if (string.IsNullOrWhiteSpace(command.Country))
            return Result<Guid>.Failure("Team country is required.");

        if (await repository.ExistsByNameAsync(command.Name, command.Country, command.Id, ct))
            return Result<Guid>.Failure("exists in database");

        var team = await repository.GetByIdAsync(command.Id, ct);
        if (team is null)
            return Result<Guid>.Failure("Team not found");

        try
        {
            team.UpdateDetails(command.Name, command.Country, command.LogoUrl, command.OfficialWebsiteUrl);
        }
        catch (ArgumentException ex)
        {
            return Result<Guid>.Failure(ex.Message);
        }

        await repository.UpdateAsync(team, ct);
        return Result<Guid>.Success(team.Id);
    }
}