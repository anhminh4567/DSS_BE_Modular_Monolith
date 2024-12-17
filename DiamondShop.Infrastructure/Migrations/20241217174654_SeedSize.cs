using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedSize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "JewelryModelCategory",
                keyColumn: "Id",
                keyValue: "3",
                columns: new[] { "Description", "Name" },
                values: new object[] { "A normal bracelet", "Bracelet" });

            migrationBuilder.InsertData(
                table: "Size",
                columns: new[] { "Id", "Unit", "Value" },
                values: new object[,]
                {
                    { "1", "milimeter", 1f },
                    { "2", "milimeter", 2f },
                    { "25", "milimeter", 25f },
                    { "26", "milimeter", 26f },
                    { "27", "milimeter", 27f },
                    { "28", "milimeter", 28f },
                    { "29", "milimeter", 29f },
                    { "30", "milimeter", 30f },
                    { "31", "milimeter", 31f },
                    { "32", "milimeter", 32f },
                    { "33", "milimeter", 33f },
                    { "34", "milimeter", 34f },
                    { "35", "milimeter", 35f },
                    { "36", "milimeter", 36f },
                    { "37", "milimeter", 37f },
                    { "38", "milimeter", 38f },
                    { "39", "milimeter", 39f },
                    { "40", "milimeter", 40f },
                    { "400", "centimeter", 40f },
                    { "41", "milimeter", 41f },
                    { "410", "centimeter", 41f },
                    { "42", "milimeter", 42f },
                    { "420", "centimeter", 42f },
                    { "43", "milimeter", 43f },
                    { "430", "centimeter", 43f },
                    { "44", "milimeter", 44f },
                    { "440", "centimeter", 44f },
                    { "45", "milimeter", 45f },
                    { "450", "centimeter", 45f },
                    { "46", "milimeter", 46f },
                    { "47", "milimeter", 47f },
                    { "48", "milimeter", 48f },
                    { "49", "milimeter", 49f },
                    { "50", "milimeter", 50f },
                    { "51", "milimeter", 51f },
                    { "52", "milimeter", 52f },
                    { "53", "milimeter", 53f },
                    { "54", "milimeter", 54f },
                    { "55", "milimeter", 55f }
                });

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 12, 17, 17, 46, 52, 80, DateTimeKind.Utc).AddTicks(2899));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 12, 17, 17, 46, 52, 80, DateTimeKind.Utc).AddTicks(3255));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreateDate",
                value: new DateTime(2024, 12, 17, 17, 46, 52, 80, DateTimeKind.Utc).AddTicks(3261));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreateDate",
                value: new DateTime(2024, 12, 17, 17, 46, 52, 80, DateTimeKind.Utc).AddTicks(3264));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "25");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "26");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "27");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "28");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "29");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "30");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "31");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "32");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "33");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "34");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "35");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "36");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "37");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "38");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "39");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "40");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "400");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "41");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "410");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "42");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "420");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "43");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "430");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "44");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "440");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "45");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "450");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "46");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "47");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "48");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "49");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "50");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "51");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "52");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "53");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "54");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "55");

            migrationBuilder.UpdateData(
                table: "JewelryModelCategory",
                keyColumn: "Id",
                keyValue: "3",
                columns: new[] { "Description", "Name" },
                values: new object[] { "A normal bracelace", "Bracelace" });

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 12, 12, 5, 56, 2, 81, DateTimeKind.Utc).AddTicks(1471));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 12, 12, 5, 56, 2, 81, DateTimeKind.Utc).AddTicks(1680));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreateDate",
                value: new DateTime(2024, 12, 12, 5, 56, 2, 81, DateTimeKind.Utc).AddTicks(1686));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreateDate",
                value: new DateTime(2024, 12, 12, 5, 56, 2, 81, DateTimeKind.Utc).AddTicks(1688));
        }
    }
}
