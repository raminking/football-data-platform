using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.Common;
using MediatR;

namespace FootballDataPlatform.Application.Competitions;

public record UpdateCompetitionCommand(Guid Id, string Name, string Country, string Code) : IRequest<Result>;

internal sealed class UpdateCompetitionHandler(ICompetitionRepository repository)
    : IRequestHandler<UpdateCompetitionCommand, Result>
{
    public async Task<Result> Handle(UpdateCompetitionCommand command, CancellationToken ct)
    {
        var competition = await repository.GetByIdAsync(command.Id, ct);
        if (competition is null) return Result.Failure("Competition not found.");

        var name = command.Name.Trim();
        var country = command.Country.Trim();
        var code = command.Code.Trim().ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(country) || string.IsNullOrWhiteSpace(code))
            return Result.Failure("Competition name, country and code are required.");

        if (await repository.ExistsByIdentityAsync(name, country, code, command.Id, ct))
            return Result.Failure("Competition already exists.");

        competition.UpdateDetails(name, country, code);
        await repository.UpdateAsync(competition, ct);
        return Result.Success();
    }
}