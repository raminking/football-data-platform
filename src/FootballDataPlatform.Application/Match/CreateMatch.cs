using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.Common;
using FootballDataPlatform.Domain.Match;
using MediatR;

namespace FootballDataPlatform.Application.Match;

public record CreateMatchCommand(Guid SeasonPublicId, Guid HomeTeamPublicId, Guid AwayTeamPublicId, DateTimeOffset ScheduledAt, MatchStage Stage, MatchStatus Status,
    int? HomeScore = null, int? AwayScore = null, int? HalfTimeHomeScore = null, int? HalfTimeAwayScore = null) : IRequest<Result<Guid>>;

internal sealed class CreateMatchHandler(IMatchRepository repository) : IRequestHandler<CreateMatchCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateMatchCommand command, CancellationToken ct)
    {
        if (command.SeasonPublicId == Guid.Empty) return Result<Guid>.Failure("Season is required.");
        if (command.HomeTeamPublicId == Guid.Empty) return Result<Guid>.Failure("Home team is required.");
        if (command.AwayTeamPublicId == Guid.Empty) return Result<Guid>.Failure("Away team is required.");
        if (command.HomeTeamPublicId == command.AwayTeamPublicId) return Result<Guid>.Failure("Home and away teams must be different.");
        var seasonId = await repository.GetSeasonIdByPublicIdAsync(command.SeasonPublicId, ct);
        var homeTeamId = await repository.GetTeamIdByPublicIdAsync(command.HomeTeamPublicId, ct);
        var awayTeamId = await repository.GetTeamIdByPublicIdAsync(command.AwayTeamPublicId, ct);
        if (!seasonId.HasValue) return Result<Guid>.Failure("Season not found.");
        if (!homeTeamId.HasValue) return Result<Guid>.Failure("Home team not found.");
        if (!awayTeamId.HasValue) return Result<Guid>.Failure("Away team not found.");
        try
        {
            var match = new Domain.Match.Match(seasonId.Value, homeTeamId.Value, awayTeamId.Value, command.ScheduledAt, command.Stage, command.Status,
                command.HomeScore, command.AwayScore, command.HalfTimeHomeScore, command.HalfTimeAwayScore);
            await repository.CreateAsync(match, ct);
            return Result<Guid>.Success(match.PublicId);
        }
        catch (ArgumentException ex) { return Result<Guid>.Failure(ex.Message); }
    }
}