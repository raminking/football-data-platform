using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballDataPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Teams",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OfficialWebsiteUrl",
                table: "Teams",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "OfficialWebsiteUrl",
                table: "Teams");
        }
    }
}
