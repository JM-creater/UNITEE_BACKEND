using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UNITEE_BACKEND.Migrations
{
    /// <inheritdoc />
    public partial class CreateProductType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ProductTypes",
                keyColumn: "ProductTypeId",
                keyValue: 4,
                column: "Product_Type",
                value: "PE Uniform");

            migrationBuilder.InsertData(
                table: "ProductTypes",
                columns: new[] { "ProductTypeId", "Product_Type" },
                values: new object[] { 5, "ID Sling" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductTypes",
                keyColumn: "ProductTypeId",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "ProductTypes",
                keyColumn: "ProductTypeId",
                keyValue: 4,
                column: "Product_Type",
                value: "ID Sling");
        }
    }
}
