using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedWarranty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "30b30471-7b09-4be6-b8bc-78fa1e05a008");

            migrationBuilder.DeleteData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "a67a5c54-6db2-47a1-8291-a965d9fc3e6d");

            migrationBuilder.InsertData(
                table: "Warranty",
                columns: new[] { "Id", "Code", "CreateDate", "MonthDuration", "Name", "Price", "Type" },
                values: new object[,]
                {
                    { "1", "THREE_MONTHS", new DateTime(2024, 10, 16, 17, 0, 0, 0, DateTimeKind.Utc), 3, "Default_Jewelry_Warranty", 0m, "0" },
                    { "2", "THREE_MONTHS", new DateTime(2024, 10, 16, 17, 0, 0, 0, DateTimeKind.Utc), 3, "Default_Diamond_Warranty", 0m, "0" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.InsertData(
                table: "Warranty",
                columns: new[] { "Id", "Code", "CreateDate", "MonthDuration", "Name", "Price", "Type" },
                values: new object[,]
                {
                    { "30b30471-7b09-4be6-b8bc-78fa1e05a008", "THREE_MONTHS", new DateTime(2024, 10, 16, 17, 0, 0, 0, DateTimeKind.Utc), 3, "Default_Jewelry_Warranty", 0m, "0" },
                    { "a67a5c54-6db2-47a1-8291-a965d9fc3e6d", "THREE_MONTHS", new DateTime(2024, 10, 16, 17, 0, 0, 0, DateTimeKind.Utc), 3, "Default_Diamond_Warranty", 0m, "0" }
                });
        }
    }
}
