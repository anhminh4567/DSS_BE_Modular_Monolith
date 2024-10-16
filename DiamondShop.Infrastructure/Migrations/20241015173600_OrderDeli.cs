using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OrderDeli : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "35be9da2-258c-44f8-b2c4-ccbd55fc636a");

            migrationBuilder.DeleteData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "9a041044-4385-40fb-a0dc-e2f604b70584");

            migrationBuilder.InsertData(
                table: "Warranty",
                columns: new[] { "Id", "Code", "CreateDate", "MonthDuration", "Name", "Price", "Type" },
                values: new object[,]
                {
                    { "17f80f8c-f625-4e3b-b1e9-68594f3d42e9", "THREE_MONTHS", new DateTime(2024, 10, 15, 17, 0, 0, 0, DateTimeKind.Utc), 3, "Default_Jewelry_Warranty", 0m, "0" },
                    { "60178ba6-ceaa-4d77-b0b9-c5f0fef13478", "THREE_MONTHS", new DateTime(2024, 10, 15, 17, 0, 0, 0, DateTimeKind.Utc), 3, "Default_Diamond_Warranty", 0m, "0" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                    { "35be9da2-258c-44f8-b2c4-ccbd55fc636a", "THREE_MONTHS", new DateTime(2024, 10, 14, 17, 0, 0, 0, DateTimeKind.Utc), 3, "Default_Diamond_Warranty", 0m, "0" },
                    { "9a041044-4385-40fb-a0dc-e2f604b70584", "THREE_MONTHS", new DateTime(2024, 10, 14, 17, 0, 0, 0, DateTimeKind.Utc), 3, "Default_Jewelry_Warranty", 0m, "0" }
                });
        }
    }
}
