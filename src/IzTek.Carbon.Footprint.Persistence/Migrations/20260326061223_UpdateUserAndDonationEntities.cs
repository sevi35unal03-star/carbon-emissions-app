using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IzTek.Carbon.Footprint.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserAndDonationEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Goals");

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "PollOptions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Message",
                table: "PollOptions");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Goals",
                type: "uuid",
                nullable: true);
        }
    }
}
