using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.Common;
using FootballDataPlatform.Domain.Teams;
using MediatR;

namespace FootballDataPlatform.Application.Teams;

public record CreateTeamCommand(string Name, string Country, string? LogoUrl = null, string? OfficialWebsiteUrl = null) : IRequest<Result<Guid>>;

internal class CreateTeamHandler(ITeamRepository repository) : IRequestHandler<CreateTeamCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateTeamCommand command, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(command.Name)) return Result<Guid>.Failure("Team name is required.");
        if (string.IsNullOrWhiteSpace(command.Country)) return Result<Guid>.Failure("Team country is required.");
        var name = command.Name.Trim();
        var country = command.Country.Trim();
        if (await repository.ExistsByNameAsync(name, country, null, ct)) return Result<Guid>.Failure("exists in database");
        try
        {
            var team = new Team(name, country, command.LogoUrl, command.OfficialWebsiteUrl);
            await repository.CreateAsync(team, ct);
            return Result<Guid>.Success(team.PublicId);
        }
        catch (ArgumentException ex) { return Result<Guid>.Failure(ex.Message); }
    }
}