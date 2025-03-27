using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace M241.Server.Migrations
{
    /// <inheritdoc />
    public partial class Mac : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MACAddr",
                table: "ClientDevice",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MACAddr",
                table: "ClientDevice");
        }
    }
}
