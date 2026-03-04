using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OsuCzechiaBot.Migrations
{
    /// <inheritdoc />
    public partial class AddReactionRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationSettings",
                schema: "osu_czechia",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Value = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReactionRoles",
                schema: "osu_czechia",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    EmojiName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    EmojiId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    IsAnimated = table.Column<bool>(type: "boolean", nullable: false),
                    IsUnicode = table.Column<bool>(type: "boolean", nullable: false),
                    UnicodeValue = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReactionRoles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReactionRoles_EmojiId_EmojiName",
                schema: "osu_czechia",
                table: "ReactionRoles",
                columns: new[] { "EmojiId", "EmojiName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationSettings",
                schema: "osu_czechia");

            migrationBuilder.DropTable(
                name: "ReactionRoles",
                schema: "osu_czechia");
        }
    }
}
