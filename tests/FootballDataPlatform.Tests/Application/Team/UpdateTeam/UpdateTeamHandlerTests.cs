using FootballDataPlatform.Application.Abstractions.Persistence;
using FootballDataPlatform.Application.Teams;
using Moq;
// اضافه کردن_alias_ برای رفع ابهام
using TeamEntity = FootballDataPlatform.Domain.Teams.Team;

namespace FootballDataPlatform.Tests.Application.Team.UpdateTeam;

public class UpdateTeamHandlerTests
{
    private readonly Mock<ITeamRepository> _mockRepository;
    private readonly UpdateTeamHandler _handler;

    public UpdateTeamHandlerTests()
    {
        _mockRepository = new Mock<ITeamRepository>();
        _handler = new UpdateTeamHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_TeamNotFound_ReturnsFailure()
    {
        var command = new UpdateTeamCommand(Guid.NewGuid(), "New Name", "New Country");
        
        // استفاده از TeamEntity به جای Team
        _mockRepository.Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((TeamEntity?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Team not found", result.Error);
    }

    [Fact]
    public async Task Handle_DuplicateNameExists_ReturnsFailure()
    {
        var teamId = Guid.NewGuid();
        // استفاده از TeamEntity
        var existingTeam = new TeamEntity("Old Name", "Old Country");
        
        _mockRepository.Setup(r => r.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(existingTeam);
        
        _mockRepository.Setup(r => r.ExistsByNameAsync("New Name", "New Country", teamId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(true);

        var command = new UpdateTeamCommand(teamId, "New Name", "New Country");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("exists", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_ValidUpdate_CallsRepositoryUpdate()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var team = new TeamEntity("Old Name", "Old Country");
        // توجه: team.Id با teamId متفاوت است چون در کانستراکتور ساخته شده
    
        _mockRepository.Setup(r => r.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);
    
        _mockRepository.Setup(r => r.ExistsByNameAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new UpdateTeamCommand(teamId, "New Name", "New Country");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    
        // اصلاح: باید چک کنیم که نتیجه برابر با ID خودِ تیم باشد (نه متغیر teamId)
        // چون هندلر team.Id را برمی‌گرداند
        Assert.Equal(team.Id, result.Value); 
    
        _mockRepository.Verify(r => r.UpdateAsync(team, It.IsAny<CancellationToken>()), Times.Once);
    
        Assert.Equal("New Name", team.Name);
        Assert.Equal("New Country", team.Country);
    }
}