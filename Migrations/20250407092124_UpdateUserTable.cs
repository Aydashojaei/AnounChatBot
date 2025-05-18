using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnounChatBot.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsInChat",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsWating",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "PartnerId",
                table: "Users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInChat",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsWating",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                table: "Users");
        }
    }
}
