using FootballDataPlatform.Application.Abstractions.ExternalData;
using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.ExternalData;
using Moq;

namespace FootballDataPlatform.Tests.Application.ExternalData;

public sealed class FootballDataImportOrchestratorTests
{
    [Fact]
    public async Task ImportCompetitionAsync_ExecutesImportServicesInOrderInsideOneTransaction()
    {
        var source = "football-data.org";
        var competition = new Mock<ICompetitionImportService>();
        var season = new Mock<ISeasonImportService>();
        var teams = new Mock<ITeamImportService>();
        var matches = new Mock<IMatchImportService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        competition.Setup(x => x.ImportAsync(source, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CompetitionImportResult(1, 0, 0, []));
        season.Setup(x => x.ImportAsync(source, "PL", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SeasonImportResult(1, 0, 0, []));
        teams.Setup(x => x.ImportAsync(source, "PL", 2025, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TeamImportResult(20, 0, 0, []));
        matches.Setup(x => x.ImportAsync(source, "PL", 2025, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MatchImportResult(380, 0, 0, []));

        unitOfWork.Setup(x => x.ExecuteInTransactionAsync(
                It.IsAny<Func<Task>>(),
                It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>((operation, _) => operation());

        var orchestrator = new FootballDataImportOrchestrator(
            competition.Object,
            season.Object,
            teams.Object,
            matches.Object,
            unitOfWork.Object);

        var result = await orchestrator.ImportCompetitionAsync(source, "PL", 2025);

        Assert.Equal(402, result.Created);
        Assert.Equal(402, result.Processed);
        Assert.Empty(result.Errors);
        unitOfWork.Verify(x => x.ExecuteInTransactionAsync(
            It.IsAny<Func<Task>>(),
            It.IsAny<CancellationToken>()), Times.Once);
        competition.VerifyAll();
        season.VerifyAll();
        teams.VerifyAll();
        matches.VerifyAll();
    }

    [Fact]
    public async Task ImportCompetitionAsync_WhenStageFails_PropagatesFailureAndDoesNotContinue()
    {
        var competition = new Mock<ICompetitionImportService>();
        var season = new Mock<ISeasonImportService>();
        var teams = new Mock<ITeamImportService>();
        var matches = new Mock<IMatchImportService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        competition.Setup(x => x.ImportAsync("football-data.org", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CompetitionImportResult(1, 0, 0, []));
        season.Setup(x => x.ImportAsync("football-data.org", "PL", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Simulated import failure."));
        unitOfWork.Setup(x => x.ExecuteInTransactionAsync(
                It.IsAny<Func<Task>>(),
                It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>((operation, _) => operation());

        var orchestrator = new FootballDataImportOrchestrator(
            competition.Object,
            season.Object,
            teams.Object,
            matches.Object,
            unitOfWork.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            orchestrator.ImportCompetitionAsync("football-data.org", "PL", 2025));

        teams.Verify(x => x.ImportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        matches.Verify(x => x.ImportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.ExecuteInTransactionAsync(
            It.IsAny<Func<Task>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ImportCompetitionAsync_WhenCancelled_DoesNotContinueToNextStage()
    {
        using var cts = new CancellationTokenSource();
        var competition = new Mock<ICompetitionImportService>();
        var season = new Mock<ISeasonImportService>();
        var teams = new Mock<ITeamImportService>();
        var matches = new Mock<IMatchImportService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        competition.Setup(x => x.ImportAsync("football-data.org", It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                cts.Cancel();
                return new CompetitionImportResult(1, 0, 0, []);
            });
        unitOfWork.Setup(x => x.ExecuteInTransactionAsync(
                It.IsAny<Func<Task>>(),
                It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>((operation, _) => operation());

        var orchestrator = new FootballDataImportOrchestrator(
            competition.Object,
            season.Object,
            teams.Object,
            matches.Object,
            unitOfWork.Object);

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            orchestrator.ImportCompetitionAsync("football-data.org", "PL", 2025, cts.Token));

        season.Verify(x => x.ImportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        teams.Verify(x => x.ImportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        matches.Verify(x => x.ImportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
