using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.Common;
using MediatR;

namespace FootballDataPlatform.Application.Competitions;

public record DeleteCompetitionCommand(Guid Id) : IRequest<Result>;

internal sealed class DeleteCompetitionHandler(ICompetitionRepository repository)
    : IRequestHandler<DeleteCompetitionCommand, Result>
{
    public async Task<Result> Handle(DeleteCompetitionCommand command, CancellationToken ct)
    {
        var competition = await repository.GetByIdAsync(command.Id, ct);
        if (competition is null) return Result.Failure("Competition not found.");

        await repository.DeleteAsync(competition, ct);
        return Result.Success();
    }
}