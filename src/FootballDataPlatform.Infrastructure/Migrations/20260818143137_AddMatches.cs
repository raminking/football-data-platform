using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballDataPlatform.Infrastructure.Migrations
{
    public partial class AddMatches : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: false),
                    HomeTeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    AwayTeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduledAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Stage = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    HomeScore = table.Column<int>(type: "integer", nullable: true),
                    AwayScore = table.Column<int>(type: "integer", nullable: true),
                    HalfTimeHomeScore = table.Column<int>(type: "integer", nullable: true),
                    HalfTimeAwayScore = table.Column<int>(type: "integer", nullable: true),
                    Result = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                    table.ForeignKey("FK_Matches_Seasons_SeasonId", x => x.SeasonId, "Seasons", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_Matches_Teams_HomeTeamId", x => x.HomeTeamId, "Teams", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_Matches_Teams_AwayTeamId", x => x.AwayTeamId, "Teams", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex("IX_Matches_SeasonId_ScheduledAt", "Matches", new[] { "SeasonId", "ScheduledAt" });
            migrationBuilder.CreateIndex("IX_Matches_HomeTeamId", "Matches", "HomeTeamId");
            migrationBuilder.CreateIndex("IX_Matches_AwayTeamId", "Matches", "AwayTeamId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Matches");
        }
    }
}