using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.Common;
using MediatR;

namespace FootballDataPlatform.Application.Competitions;

public record DeleteCompetitionCommand(Guid Id) : IRequest<Result<Guid>>;

internal sealed class DeleteCompetitionHandler(ICompetitionRepository repository)
    : IRequestHandler<DeleteCompetitionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(DeleteCompetitionCommand command, CancellationToken ct)
    {
        var competition = await repository.GetByIdAsync(command.Id, ct);
        if (competition is null) return Result<Guid>.Failure("Competition not found.");

        await repository.DeleteAsync(competition, ct);
        return Result<Guid>.Success(command.Id);
    }
}