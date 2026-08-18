using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.Common;
using FootballDataPlatform.Domain.Match;
using MediatR;

namespace FootballDataPlatform.Application.Match;

public record UpdateMatchCommand(
    Guid Id,
    DateTimeOffset ScheduledAt,
    MatchStage Stage,
    MatchStatus Status,
    int? HomeScore = null,
    int? AwayScore = null,
    int? HalfTimeHomeScore = null,
    int? HalfTimeAwayScore = null) : IRequest<Result<bool>>;

internal sealed class UpdateMatchHandler(IMatchRepository repository)
    : IRequestHandler<UpdateMatchCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateMatchCommand command, CancellationToken ct)
    {
        var match = await repository.GetByIdAsync(command.Id, ct);
        if (match is null) return Result<bool>.Failure("Match not found.");

        try
        {
            match.UpdateDetails(
                command.ScheduledAt,
                command.Stage,
                command.Status,
                command.HomeScore,
                command.AwayScore,
                command.HalfTimeHomeScore,
                command.HalfTimeAwayScore);

            await repository.UpdateAsync(match, ct);
            return Result<bool>.Success(true);
        }
        catch (ArgumentException ex)
        {
            return Result<bool>.Failure(ex.Message);
        }
    }
}