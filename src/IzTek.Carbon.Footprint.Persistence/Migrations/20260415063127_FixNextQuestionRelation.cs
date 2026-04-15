using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IzTek.Carbon.Footprint.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixNextQuestionRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityOptions_ActivityQuestions_NextQuestionId",
                table: "ActivityOptions");

            migrationBuilder.DropIndex(
                name: "IX_ActivityOptions_NextQuestionId",
                table: "ActivityOptions");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityOptions_NextQuestionId",
                table: "ActivityOptions",
                column: "NextQuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityOptions_ActivityQuestions_NextQuestionId",
                table: "ActivityOptions",
                column: "NextQuestionId",
                principalTable: "ActivityQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityOptions_ActivityQuestions_NextQuestionId",
                table: "ActivityOptions");

            migrationBuilder.DropIndex(
                name: "IX_ActivityOptions_NextQuestionId",
                table: "ActivityOptions");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityOptions_NextQuestionId",
                table: "ActivityOptions",
                column: "NextQuestionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityOptions_ActivityQuestions_NextQuestionId",
                table: "ActivityOptions",
                column: "NextQuestionId",
                principalTable: "ActivityQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
