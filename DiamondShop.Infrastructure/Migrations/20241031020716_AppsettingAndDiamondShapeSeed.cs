using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AppsettingAndDiamondShapeSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductLock_AccountId",
                table: "Diamond",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProductLock_LockEndDate",
                table: "Diamond",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "Address",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ApplicationSettings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationSettings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Diamond_Shape",
                columns: new[] { "Id", "Shape" },
                values: new object[,]
                {
                    { "98", "Fancy_Shape" },
                    { "99", "Any" }
                });

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 10, 30, 17, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 10, 30, 17, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.CreateIndex(
                name: "IX_DiamondCriteria_CaratFrom_CaratTo_IsSideDiamond_IsLabGrown",
                table: "DiamondCriteria",
                columns: new[] { "CaratFrom", "CaratTo", "IsSideDiamond", "IsLabGrown" });

            migrationBuilder.CreateIndex(
                name: "IX_Diamond_Carat_Color_Clarity_Cut_IsLabDiamond_JewelryId",
                table: "Diamond",
                columns: new[] { "Carat", "Color", "Clarity", "Cut", "IsLabDiamond", "JewelryId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationSettings");

            migrationBuilder.DropIndex(
                name: "IX_DiamondCriteria_CaratFrom_CaratTo_IsSideDiamond_IsLabGrown",
                table: "DiamondCriteria");

            migrationBuilder.DropIndex(
                name: "IX_Diamond_Carat_Color_Clarity_Cut_IsLabDiamond_JewelryId",
                table: "Diamond");

            migrationBuilder.DeleteData(
                table: "Diamond_Shape",
                keyColumn: "Id",
                keyValue: "98");

            migrationBuilder.DeleteData(
                table: "Diamond_Shape",
                keyColumn: "Id",
                keyValue: "99");

            migrationBuilder.DropColumn(
                name: "ProductLock_AccountId",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "ProductLock_LockEndDate",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Address");

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 10, 29, 17, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 10, 29, 17, 0, 0, 0, DateTimeKind.Utc));
        }
    }
}
