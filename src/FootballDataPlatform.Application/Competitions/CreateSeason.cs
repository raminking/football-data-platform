using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.Common;
using FootballDataPlatform.Domain.Competitions;
using MediatR;

namespace FootballDataPlatform.Application.Competitions;

public record CreateSeasonCommand(Guid CompetitionPublicId, string Name, DateOnly StartDate, DateOnly EndDate) : IRequest<Result<Guid>>;
internal sealed class CreateSeasonHandler(ISeasonRepository seasonRepository, ICompetitionRepository competitionRepository) : IRequestHandler<CreateSeasonCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateSeasonCommand command, CancellationToken ct)
    {
        if (command.CompetitionPublicId == Guid.Empty) return Result<Guid>.Failure("Competition is required.");
        if (string.IsNullOrWhiteSpace(command.Name)) return Result<Guid>.Failure("Season name is required.");
        if (command.EndDate < command.StartDate) return Result<Guid>.Failure("Season end date cannot be before start date.");
        var competition = await competitionRepository.GetByPublicIdAsync(command.CompetitionPublicId, ct);
        if (competition is null) return Result<Guid>.Failure("Competition not found.");
        var name = command.Name.Trim();
        if (await seasonRepository.ExistsByIdentityAsync(competition.Id, name, null, ct)) return Result<Guid>.Failure("Season already exists for this competition.");
        var season = new Season(competition.Id, name, command.StartDate, command.EndDate);
        await seasonRepository.CreateAsync(season, ct);
        return Result<Guid>.Success(season.PublicId);
    }
}