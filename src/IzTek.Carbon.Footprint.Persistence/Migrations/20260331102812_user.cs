using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IzTek.Carbon.Footprint.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class user : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Kolon zaten varsa atla
            migrationBuilder.Sql(@"
        ALTER TABLE ""UserPollResults"" 
        ADD COLUMN IF NOT EXISTS ""IsCompleted"" boolean NOT NULL DEFAULT false;
    ");
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
