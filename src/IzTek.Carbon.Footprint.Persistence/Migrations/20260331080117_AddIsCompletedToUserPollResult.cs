using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IzTek.Carbon.Footprint.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIsCompletedToUserPollResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "UserPollResults",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "UserPollResults");
        }
    }
}
