using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace M241.Server.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AQI",
                table: "RoomData");

            migrationBuilder.DropColumn(
                name: "NO2",
                table: "RoomData");

            migrationBuilder.DropColumn(
                name: "O3",
                table: "RoomData");

            migrationBuilder.AlterColumn<float>(
                name: "Temperature",
                table: "RoomData",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<float>(
                name: "Humidity",
                table: "RoomData",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<float>(
                name: "Gas",
                table: "RoomData",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Pressure",
                table: "RoomData",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gas",
                table: "RoomData");

            migrationBuilder.DropColumn(
                name: "Pressure",
                table: "RoomData");

            migrationBuilder.AlterColumn<int>(
                name: "Temperature",
                table: "RoomData",
                type: "integer",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<int>(
                name: "Humidity",
                table: "RoomData",
                type: "integer",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AddColumn<int>(
                name: "AQI",
                table: "RoomData",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NO2",
                table: "RoomData",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "O3",
                table: "RoomData",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
