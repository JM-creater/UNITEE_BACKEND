using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UNITEE_BACKEND.Migrations
{
    /// <inheritdoc />
    public partial class AddedPriceTotal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PriceTotal",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceTotal",
                table: "Products");
        }
    }
}
