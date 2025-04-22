using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace M241.Server.Migrations
{
    /// <inheritdoc />
    public partial class RoomIsBurning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBurning",
                table: "Rooms",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBurning",
                table: "Rooms");
        }
    }
}
