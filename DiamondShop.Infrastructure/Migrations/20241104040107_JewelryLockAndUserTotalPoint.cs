using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class JewelryLockAndUserTotalPoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Diamond_Jewelry_JewelryId",
                table: "Diamond");

            migrationBuilder.AddColumn<string>(
                name: "ProductLock_AccountId",
                table: "Jewelry",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProductLock_LockEndDate",
                table: "Jewelry",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPoint",
                table: "Account",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 11, 3, 17, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 11, 3, 17, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.InsertData(
                table: "Warranty",
                columns: new[] { "Id", "Code", "CreateDate", "MonthDuration", "Name", "Price", "Type" },
                values: new object[,]
                {
                    { "3", "ONE_YEAR", new DateTime(2024, 11, 3, 17, 0, 0, 0, DateTimeKind.Utc), 12, "One_Year_Jewelry_Warranty", 150000m, "Jewelry" },
                    { "4", "ONE_YEAR", new DateTime(2024, 11, 3, 17, 0, 0, 0, DateTimeKind.Utc), 12, "One_Year_Diamond_Warranty", 120000m, "Diamond" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Diamond_Jewelry_JewelryId",
                table: "Diamond",
                column: "JewelryId",
                principalTable: "Jewelry",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Diamond_Jewelry_JewelryId",
                table: "Diamond");

            migrationBuilder.DeleteData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3");

            migrationBuilder.DeleteData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4");

            migrationBuilder.DropColumn(
                name: "ProductLock_AccountId",
                table: "Jewelry");

            migrationBuilder.DropColumn(
                name: "ProductLock_LockEndDate",
                table: "Jewelry");

            migrationBuilder.DropColumn(
                name: "TotalPoint",
                table: "Account");

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 11, 1, 17, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 11, 1, 17, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddForeignKey(
                name: "FK_Diamond_Jewelry_JewelryId",
                table: "Diamond",
                column: "JewelryId",
                principalTable: "Jewelry",
                principalColumn: "Id");
        }
    }
}
