using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModelPriceOffset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "10c26601-ee6f-4e37-ab7b-52e89d4b0416");

            migrationBuilder.DeleteData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "6fa48c71-8a15-4fdb-9771-0a36d09fbee3");

            migrationBuilder.AddColumn<decimal>(
                name: "PriceOffset",
                table: "JewelryModel",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.InsertData(
                table: "Warranty",
                columns: new[] { "Id", "Code", "CreateDate", "MonthDuration", "Name", "Price", "Type" },
                values: new object[,]
                {
                    { "060a6f41-2a58-4580-bd0e-174cc052c6a6", "THREE_MONTHS", new DateTime(2024, 10, 15, 17, 0, 0, 0, DateTimeKind.Utc), 3, "Default_Diamond_Warranty", 0m, "0" },
                    { "db4e6535-19ac-42f8-bcc7-37ea9befcbd2", "THREE_MONTHS", new DateTime(2024, 10, 15, 17, 0, 0, 0, DateTimeKind.Utc), 3, "Default_Jewelry_Warranty", 0m, "0" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "060a6f41-2a58-4580-bd0e-174cc052c6a6");

            migrationBuilder.DeleteData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "db4e6535-19ac-42f8-bcc7-37ea9befcbd2");

            migrationBuilder.DropColumn(
                name: "PriceOffset",
                table: "JewelryModel");

            migrationBuilder.InsertData(
                table: "Warranty",
                columns: new[] { "Id", "Code", "CreateDate", "MonthDuration", "Name", "Price", "Type" },
                values: new object[,]
                {
                    { "10c26601-ee6f-4e37-ab7b-52e89d4b0416", "THREE_MONTHS", new DateTime(2024, 10, 15, 17, 0, 0, 0, DateTimeKind.Utc), 3, "Default_Jewelry_Warranty", 0m, "0" },
                    { "6fa48c71-8a15-4fdb-9771-0a36d09fbee3", "THREE_MONTHS", new DateTime(2024, 10, 15, 17, 0, 0, 0, DateTimeKind.Utc), 3, "Default_Diamond_Warranty", 0m, "0" }
                });
        }
    }
}
