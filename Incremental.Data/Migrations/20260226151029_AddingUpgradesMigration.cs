using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Incremental.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingUpgradesMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClickPower",
                table: "Points");

            migrationBuilder.DropColumn(
                name: "PassiveIncome",
                table: "Points");

            migrationBuilder.DropColumn(
                name: "PassiveInterval",
                table: "Points");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastPassiveTick",
                table: "Points",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateTable(
                name: "Upgrades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BaseValue = table.Column<long>(type: "bigint", nullable: false),
                    ValueMultiplier = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BasePrice = table.Column<long>(type: "bigint", nullable: false),
                    PriceMultiplier = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UpgradeType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Upgrades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerUpgrades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PointsId = table.Column<int>(type: "int", nullable: false),
                    UpgradeId = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerUpgrades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerUpgrades_Points_PointsId",
                        column: x => x.PointsId,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerUpgrades_Upgrades_UpgradeId",
                        column: x => x.UpgradeId,
                        principalTable: "Upgrades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerUpgrades_PointsId",
                table: "PlayerUpgrades",
                column: "PointsId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerUpgrades_UpgradeId",
                table: "PlayerUpgrades",
                column: "UpgradeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerUpgrades");

            migrationBuilder.DropTable(
                name: "Upgrades");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastPassiveTick",
                table: "Points",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ClickPower",
                table: "Points",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

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
    }
}
