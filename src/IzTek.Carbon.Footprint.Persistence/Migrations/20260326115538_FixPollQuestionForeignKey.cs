using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IzTek.Carbon.Footprint.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixPollQuestionForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PollQuestions_PollSets_PollSetId1",
                table: "PollQuestions");

            migrationBuilder.DropIndex(
                name: "IX_PollQuestions_PollSetId1",
                table: "PollQuestions");

            migrationBuilder.DropColumn(
                name: "PollSetId1",
                table: "PollQuestions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PollSetId1",
                table: "PollQuestions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PollQuestions_PollSetId1",
                table: "PollQuestions",
                column: "PollSetId1");

            migrationBuilder.AddForeignKey(
                name: "FK_PollQuestions_PollSets_PollSetId1",
                table: "PollQuestions",
                column: "PollSetId1",
                principalTable: "PollSets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
