using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnounChatBot.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsInChatColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInChat",
                table: "Users");

            migrationBuilder.AlterColumn<long>(
                name: "PartnerId",
                table: "Users",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "PartnerId",
                table: "Users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsInChat",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
