using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OsuCzechiaBot.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "osu_czechia");

            migrationBuilder.CreateTable(
                name: "AuthorizedUsers",
                schema: "osu_czechia",
                columns: table => new
                {
                    DiscordId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    OsuId = table.Column<int>(type: "integer", nullable: false),
                    AccessToken = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    RefreshToken = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    Expires = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CountryCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizedUsers", x => x.DiscordId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthorizedUsers",
                schema: "osu_czechia");
        }
    }
}
