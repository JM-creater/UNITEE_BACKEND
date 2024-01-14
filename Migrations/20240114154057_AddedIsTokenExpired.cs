using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UNITEE_BACKEND.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsTokenExpired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTokenExpired",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 20163482,
                column: "IsTokenExpired",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTokenExpired",
                table: "Users");
        }
    }
}
