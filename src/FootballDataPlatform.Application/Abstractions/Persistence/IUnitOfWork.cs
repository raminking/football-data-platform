namespace FootballDataPlatform.Application.Abstractions.Persistence;

public interface IUnitOfWork
{
    Task ExecuteInTransactionAsync(
        Func<Task> operation,
        CancellationToken cancellationToken = default);
}
