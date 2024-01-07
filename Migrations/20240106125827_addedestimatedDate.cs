using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UNITEE_BACKEND.Migrations
{
    /// <inheritdoc />
    public partial class addedestimatedDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EstimatedDate",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "DepartmentId", "Department_Name" },
                values: new object[] { 13, "Seacast" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 13);

            migrationBuilder.DropColumn(
                name: "EstimatedDate",
                table: "Orders");
        }
    }
}
