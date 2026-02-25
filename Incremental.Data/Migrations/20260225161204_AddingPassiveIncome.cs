using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Incremental.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingPassiveIncome : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastPassiveTick",
                table: "Points",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "PassiveIncome",
                table: "Points",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "PassiveInterval",
                table: "Points",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastPassiveTick",
                table: "Points");

            migrationBuilder.DropColumn(
                name: "PassiveIncome",
                table: "Points");

            migrationBuilder.DropColumn(
                name: "PassiveInterval",
                table: "Points");
        }
    }
}
