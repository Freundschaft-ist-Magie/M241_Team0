using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace M241.Server.Migrations
{
    /// <inheritdoc />
    public partial class RemoveClient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomData_Clients_ClientId",
                table: "RoomData");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_RoomData_ClientId",
                table: "RoomData");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "RoomData");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Rooms",
                newName: "MACAddress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MACAddress",
                table: "Rooms",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "RoomData",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    RoomId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clients_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomData_ClientId",
                table: "RoomData",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_RoomId",
                table: "Clients",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomData_Clients_ClientId",
                table: "RoomData",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id");
        }
    }
}
