using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OsuCzechiaBot.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthorizedColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Authorized",
                schema: "osu_czechia",
                table: "AuthorizedUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
            
            migrationBuilder.Sql("""UPDATE osu_czechia."AuthorizedUsers" SET "Authorized" = true;""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Authorized",
                schema: "osu_czechia",
                table: "AuthorizedUsers");
        }
    }
}
