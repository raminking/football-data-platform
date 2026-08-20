using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.Common;
using MediatR;

namespace FootballDataPlatform.Application.Competitions;

public record UpdateCompetitionCommand(Guid PublicId, string Name, string Country, string Code) : IRequest<Result<Guid>>;
internal sealed class UpdateCompetitionHandler(ICompetitionRepository repository) : IRequestHandler<UpdateCompetitionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(UpdateCompetitionCommand command, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(command.Name)) return Result<Guid>.Failure("Competition name is required.");
        if (string.IsNullOrWhiteSpace(command.Country)) return Result<Guid>.Failure("Competition country is required.");
        if (string.IsNullOrWhiteSpace(command.Code)) return Result<Guid>.Failure("Competition code is required.");
        var competition = await repository.GetByPublicIdAsync(command.PublicId, ct);
        if (competition is null) return Result<Guid>.Failure("Competition not found.");
        var name = command.Name.Trim(); var country = command.Country.Trim(); var code = command.Code.Trim().ToUpperInvariant();
        if (await repository.ExistsByIdentityAsync(name, country, code, competition.Id, ct)) return Result<Guid>.Failure("Competition already exists.");
        competition.UpdateDetails(name, country, code);
        await repository.UpdateAsync(competition, ct);
        return Result<Guid>.Success(competition.PublicId);
    }
}