using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.Common;
using FootballDataPlatform.Domain.Competitions;
using MediatR;

namespace FootballDataPlatform.Application.Competitions;

public record CreateSeasonCommand(Guid CompetitionId, string Name, DateOnly StartDate, DateOnly EndDate) : IRequest<Result<Guid>>;

internal sealed class CreateSeasonHandler(ISeasonRepository repository)
    : IRequestHandler<CreateSeasonCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateSeasonCommand command, CancellationToken ct)
    {
        if (command.CompetitionId == Guid.Empty) return Result<Guid>.Failure("Competition is required.");
        if (string.IsNullOrWhiteSpace(command.Name)) return Result<Guid>.Failure("Season name is required.");
        if (command.EndDate < command.StartDate) return Result<Guid>.Failure("Season end date cannot be before start date.");
        if (!await repository.CompetitionExistsAsync(command.CompetitionId, ct))
            return Result<Guid>.Failure("Competition not found.");

        var name = command.Name.Trim();
        if (await repository.ExistsByIdentityAsync(command.CompetitionId, name, ct))
            return Result<Guid>.Failure("Season already exists for this competition.");

        var season = new Season(command.CompetitionId, name, command.StartDate, command.EndDate);
        await repository.CreateAsync(season, ct);
        return Result<Guid>.Success(season.Id);
    }
}