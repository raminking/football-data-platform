using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.Teams;
using Moq;
// اضافه کردن alias برای رفع ابهام نام کلاس Team با namespace تست
using TeamEntity = FootballDataPlatform.Domain.Teams.Team;

namespace FootballDataPlatform.Tests.Teams.Application;

public class DeleteTeamHandlerTests
{
    private readonly Mock<ITeamRepository> _mockRepository;
    private readonly DeleteTeamHandler _handler; // تغییر به DeleteTeamHandler

    public DeleteTeamHandlerTests()
    {
        _mockRepository = new Mock<ITeamRepository>();
        _handler = new DeleteTeamHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_TeamNotFound_ReturnsFailure()
    {
        // Arrange
        var command = new DeleteTeamCommand(Guid.NewGuid()); // استفاده از DeleteTeamCommand
        
        _mockRepository.Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((TeamEntity?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Team not found", result.Error);
    }

    [Fact]
    public async Task Handle_ValidDelete_CallsRepositoryDelete()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var team = new TeamEntity("Real Madrid", "Spain");
        
        _mockRepository.Setup(r => r.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(team);

        var command = new DeleteTeamCommand(teamId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(team.Id, result.Value);
        
        // Verify that DeleteAsync was called exactly once
        _mockRepository.Verify(r => r.DeleteAsync(team, It.IsAny<CancellationToken>()), Times.Once);
    }
}