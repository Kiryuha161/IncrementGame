using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Incremental.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitMigration3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Amount",
                table: "Points",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<long>(
                name: "ClickPower",
                table: "Points",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClickPower",
                table: "Points");

            migrationBuilder.AlterColumn<int>(
                name: "Amount",
                table: "Points",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
