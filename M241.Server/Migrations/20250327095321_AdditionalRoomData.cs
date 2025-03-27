using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace M241.Server.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalRoomData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AQI",
                table: "Rooms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NO2",
                table: "Rooms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "O3",
                table: "Rooms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Temperature",
                table: "Rooms",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AQI",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "NO2",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "O3",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "Rooms");
        }
    }
}
