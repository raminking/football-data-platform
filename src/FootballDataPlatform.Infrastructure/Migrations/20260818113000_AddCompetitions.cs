using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballDataPlatform.Infrastructure.Migrations;

public partial class AddCompetitions : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Competitions",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Competitions", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Competitions_Name_Country_Code",
            table: "Competitions",
            columns: new[] { "Name", "Country", "Code" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Competitions");
    }
}