using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IzTek.Carbon.Footprint.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SyncModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PollOptions_PollQuestions_NextPollQuestionId",
                table: "PollOptions");

            migrationBuilder.DropIndex(
                name: "IX_PollOptions_NextPollQuestionId",
                table: "PollOptions");
        }
    }
}