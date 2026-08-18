using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.Common;
using FootballDataPlatform.Domain.Match;
using MediatR;

namespace FootballDataPlatform.Application.Match;

public record CreateMatchCommand(
    Guid SeasonId,
    Guid HomeTeamId,
    Guid AwayTeamId,
    DateTimeOffset ScheduledAt,
    MatchStage Stage,
    MatchStatus Status,
    int? HomeScore = null,
    int? AwayScore = null,
    int? HalfTimeHomeScore = null,
    int? HalfTimeAwayScore = null) : IRequest<Result<Guid>>;

internal sealed class CreateMatchHandler(IMatchRepository repository)
    : IRequestHandler<CreateMatchCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateMatchCommand command, CancellationToken ct)
    {
        if (command.SeasonId == Guid.Empty) return Result<Guid>.Failure("Season is required.");
        if (command.HomeTeamId == Guid.Empty) return Result<Guid>.Failure("Home team is required.");
        if (command.AwayTeamId == Guid.Empty) return Result<Guid>.Failure("Away team is required.");
        if (command.HomeTeamId == command.AwayTeamId) return Result<Guid>.Failure("Home and away teams must be different.");
        if (!await repository.SeasonExistsAsync(command.SeasonId, ct)) return Result<Guid>.Failure("Season not found.");
        if (!await repository.TeamExistsAsync(command.HomeTeamId, ct)) return Result<Guid>.Failure("Home team not found.");
        if (!await repository.TeamExistsAsync(command.AwayTeamId, ct)) return Result<Guid>.Failure("Away team not found.");

        try
        {
            var match = new FootballDataPlatform.Domain.Match.Match(
                command.SeasonId,
                command.HomeTeamId,
                command.AwayTeamId,
                command.ScheduledAt,
                command.Stage,
                command.Status,
                command.HomeScore,
                command.AwayScore,
                command.HalfTimeHomeScore,
                command.HalfTimeAwayScore);

            await repository.CreateAsync(match, ct);
            return Result<Guid>.Success(match.Id);
        }
        catch (ArgumentException ex)
        {
            return Result<Guid>.Failure(ex.Message);
        }
    }
}