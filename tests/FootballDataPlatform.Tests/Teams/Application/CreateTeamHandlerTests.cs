using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.Teams;
using Moq;

namespace FootballDataPlatform.Tests.Teams.Application;

public class CreateTeamHandlerTests
{
    private readonly Mock<ITeamRepository> _repository;
    private readonly CreateTeamHandler _handler;

    public CreateTeamHandlerTests()
    {
        _repository = new Mock<ITeamRepository>();
        _handler = new CreateTeamHandler(_repository.Object);
    }

    [Fact]
    public async Task Handle_EmptyName_ReturnsFailure()
    {
        // Arrange
        var command = new CreateTeamCommand(
            "",
            "England");

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Team name is required.",
            result.Error);

        _repository.Verify(
            r => r.CreateAsync(
                It.IsAny<FootballDataPlatform.Domain.Teams.Team>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_EmptyCountry_ReturnsFailure()
    {
        // Arrange
        var command = new CreateTeamCommand(
            "Liverpool",
            "");

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Team country is required.",
            result.Error);

        _repository.Verify(
            r => r.CreateAsync(
                It.IsAny<FootballDataPlatform.Domain.Teams.Team>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_DuplicateTeam_ReturnsFailure()
    {
        // Arrange
        var command = new CreateTeamCommand(
            "Liverpool",
            "England");

        _repository
            .Setup(r => r.ExistsByNameAsync(
                command.Name,
                command.Country,
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "exists in database",
            result.Error);

        _repository.Verify(
            r => r.CreateAsync(
                It.IsAny<FootballDataPlatform.Domain.Teams.Team>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesTeam()
    {
        // Arrange
        var command = new CreateTeamCommand(
            "Liverpool",
            "England");

        _repository
            .Setup(r => r.ExistsByNameAsync(
                command.Name,
                command.Country,
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);

        _repository.Verify(
            r => r.CreateAsync(
                It.Is<FootballDataPlatform.Domain.Teams.Team>(
                    team =>
                        team.Name == "Liverpool" &&
                        team.Country == "England"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task Handle_WhitespaceInput_TrimsValues()
    {
        // Arrange
        var command = new CreateTeamCommand(
            "  Liverpool  ",
            "  England  ");

        _repository
            .Setup(r => r.ExistsByNameAsync(
                "Liverpool",
                "England",
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        _repository.Verify(
            r => r.ExistsByNameAsync(
                "Liverpool",
                "England",
                null,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _repository.Verify(
            r => r.CreateAsync(
                It.Is<FootballDataPlatform.Domain.Teams.Team>(
                    team =>
                        team.Name == "Liverpool" &&
                        team.Country == "England"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}