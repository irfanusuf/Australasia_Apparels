using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace P2WebMVC.Migrations
{
    /// <inheritdoc />
    public partial class MIg22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResetPassToken",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetPassTokenExpiry",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResetPassToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ResetPassTokenExpiry",
                table: "Users");
        }
    }
}
