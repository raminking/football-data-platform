using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.Teams;
using Moq;

namespace FootballDataPlatform.Tests.Teams.Application;

public class GetTeamHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnTeam_WhenTeamExists()
    {
        // Arrange
        var team = new FootballDataPlatform.Domain.Teams.Team(
            "Benfica",
            "Portugal");

        var repository = new Mock<ITeamRepository>();

        repository
            .Setup(x => x.GetByIdAsync(
                team.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        var handler = new GetTeamHandler(repository.Object);

        var query = new GetTeamQuery(team.Id);

        // Act
        var result = await handler.Handle(
            query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(team.Id, result.Id);
        Assert.Equal("Benfica", result.Name);
    }
}