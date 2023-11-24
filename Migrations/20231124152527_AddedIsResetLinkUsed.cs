﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UNITEE_BACKEND.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsResetLinkUsed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsResetLinkUsed",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 20163482,
                column: "IsResetLinkUsed",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsResetLinkUsed",
                table: "Users");
        }
    }
}
