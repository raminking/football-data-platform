using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.Common;
using FootballDataPlatform.Domain.Competitions;
using MediatR;

namespace FootballDataPlatform.Application.Competitions;

public record CreateCompetitionCommand(string Name, string Country, string Code) : IRequest<Result<Guid>>;

internal sealed class CreateCompetitionHandler(ICompetitionRepository repository) : IRequestHandler<CreateCompetitionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateCompetitionCommand command, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(command.Name)) return Result<Guid>.Failure("Competition name is required.");
        if (string.IsNullOrWhiteSpace(command.Country)) return Result<Guid>.Failure("Competition country is required.");
        if (string.IsNullOrWhiteSpace(command.Code)) return Result<Guid>.Failure("Competition code is required.");
        var name = command.Name.Trim(); var country = command.Country.Trim(); var code = command.Code.Trim().ToUpperInvariant();
        if (await repository.ExistsByIdentityAsync(name, country, code, null, ct)) return Result<Guid>.Failure("Competition already exists.");
        var competition = new Competition(name, country, code);
        await repository.CreateAsync(competition, ct);
        return Result<Guid>.Success(competition.PublicId);
    }
}