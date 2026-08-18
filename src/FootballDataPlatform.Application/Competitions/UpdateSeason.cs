using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.Common;
using MediatR;

namespace FootballDataPlatform.Application.Competitions;

public record UpdateSeasonCommand(Guid Id, string Name, DateOnly StartDate, DateOnly EndDate) : IRequest<Result<bool>>;

internal sealed class UpdateSeasonHandler(ISeasonRepository repository)
    : IRequestHandler<UpdateSeasonCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateSeasonCommand command, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(command.Name)) return Result<bool>.Failure("Season name is required.");
        if (command.EndDate < command.StartDate) return Result<bool>.Failure("Season end date cannot be before start date.");

        var season = await repository.GetByIdAsync(command.Id, ct);
        if (season is null) return Result<bool>.Failure("Season not found.");

        var name = command.Name.Trim();
        if (await repository.ExistsByIdentityAsync(season.CompetitionId, name, ct) &&
            !string.Equals(season.Name, name, StringComparison.OrdinalIgnoreCase))
            return Result<bool>.Failure("Season already exists for this competition.");

        season.UpdateDetails(name, command.StartDate, command.EndDate);
        await repository.UpdateAsync(season, ct);
        return Result<bool>.Success(true);
    }
}