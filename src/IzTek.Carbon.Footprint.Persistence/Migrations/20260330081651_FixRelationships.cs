using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IzTek.Carbon.Footprint.Persistence.Migrations
{
    public partial class FixRelationships : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PollQuestionId1",
                table: "PollOptions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PollOptions_NextPollQuestionId",
                table: "PollOptions",
                column: "NextPollQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_PollOptions_PollQuestionId1",
                table: "PollOptions",
                column: "PollQuestionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_PollOptions_PollQuestions_NextPollQuestionId",
                table: "PollOptions",
                column: "NextPollQuestionId",
                principalTable: "PollQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PollOptions_PollQuestions_PollQuestionId1",
                table: "PollOptions",
                column: "PollQuestionId1",
                principalTable: "PollQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}