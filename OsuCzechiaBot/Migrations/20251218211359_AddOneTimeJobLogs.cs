using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OsuCzechiaBot.Migrations
{
    /// <inheritdoc />
    public partial class AddOneTimeJobLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DiscordId",
                schema: "osu_czechia",
                table: "AuthorizedUsers",
                newName: "Id");

            migrationBuilder.CreateTable(
                name: "OneTimeJobLogs",
                schema: "osu_czechia",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    StartTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OneTimeJobLogs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OneTimeJobLogs",
                schema: "osu_czechia");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "osu_czechia",
                table: "AuthorizedUsers",
                newName: "DiscordId");
        }
    }
}
