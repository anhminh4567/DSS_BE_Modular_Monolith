using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MainDiamondShape_ShapeId",
                table: "MainDiamondShape");

            migrationBuilder.DeleteData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "17f80f8c-f625-4e3b-b1e9-68594f3d42e9");

            migrationBuilder.DeleteData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "60178ba6-ceaa-4d77-b0b9-c5f0fef13478");

            migrationBuilder.InsertData(
                table: "Warranty",
                columns: new[] { "Id", "Code", "CreateDate", "MonthDuration", "Name", "Price", "Type" },
                values: new object[,]
                {
                    { "10c26601-ee6f-4e37-ab7b-52e89d4b0416", "THREE_MONTHS", new DateTime(2024, 10, 15, 17, 0, 0, 0, DateTimeKind.Utc), 3, "Default_Jewelry_Warranty", 0m, "0" },
                    { "6fa48c71-8a15-4fdb-9771-0a36d09fbee3", "THREE_MONTHS", new DateTime(2024, 10, 15, 17, 0, 0, 0, DateTimeKind.Utc), 3, "Default_Diamond_Warranty", 0m, "0" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MainDiamondShape_ShapeId",
                table: "MainDiamondShape",
                column: "ShapeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MainDiamondShape_ShapeId",
                table: "MainDiamondShape");

            migrationBuilder.DeleteData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "10c26601-ee6f-4e37-ab7b-52e89d4b0416");

            migrationBuilder.DeleteData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "6fa48c71-8a15-4fdb-9771-0a36d09fbee3");

            migrationBuilder.InsertData(
                table: "Warranty",
                columns: new[] { "Id", "Code", "CreateDate", "MonthDuration", "Name", "Price", "Type" },
                values: new object[,]
                {
                    { "17f80f8c-f625-4e3b-b1e9-68594f3d42e9", "THREE_MONTHS", new DateTime(2024, 10, 15, 17, 0, 0, 0, DateTimeKind.Utc), 3, "Default_Jewelry_Warranty", 0m, "0" },
                    { "60178ba6-ceaa-4d77-b0b9-c5f0fef13478", "THREE_MONTHS", new DateTime(2024, 10, 15, 17, 0, 0, 0, DateTimeKind.Utc), 3, "Default_Diamond_Warranty", 0m, "0" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MainDiamondShape_ShapeId",
                table: "MainDiamondShape",
                column: "ShapeId",
                unique: true);
        }
    }
}
