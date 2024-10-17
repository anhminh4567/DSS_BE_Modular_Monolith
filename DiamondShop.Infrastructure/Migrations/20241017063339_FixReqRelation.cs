using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixReqRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SideDiamondReq_ShapeId",
                table: "SideDiamondReq");

            migrationBuilder.DeleteData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "060a6f41-2a58-4580-bd0e-174cc052c6a6");

            migrationBuilder.DeleteData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "db4e6535-19ac-42f8-bcc7-37ea9befcbd2");

            migrationBuilder.InsertData(
                table: "JewelryModelCategory",
                columns: new[] { "Id", "Description", "IsGeneral", "Name", "ParentCategoryId", "ThumbnailPath" },
                values: new object[,]
                {
                    { "1", "A normal ring", true, "Ring", null, "" },
                    { "2", "A normal necklace", true, "Necklace", null, "" },
                    { "3", "A normal bracelace", true, "Bracelace", null, "" },
                    { "4", "A normal earring", true, "Earring", null, "" }
                });

            migrationBuilder.InsertData(
                table: "Warranty",
                columns: new[] { "Id", "Code", "CreateDate", "MonthDuration", "Name", "Price", "Type" },
                values: new object[,]
                {
                    { "30b30471-7b09-4be6-b8bc-78fa1e05a008", "THREE_MONTHS", new DateTime(2024, 10, 16, 17, 0, 0, 0, DateTimeKind.Utc), 3, "Default_Jewelry_Warranty", 0m, "0" },
                    { "a67a5c54-6db2-47a1-8291-a965d9fc3e6d", "THREE_MONTHS", new DateTime(2024, 10, 16, 17, 0, 0, 0, DateTimeKind.Utc), 3, "Default_Diamond_Warranty", 0m, "0" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SideDiamondReq_ShapeId",
                table: "SideDiamondReq",
                column: "ShapeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SideDiamondReq_ShapeId",
                table: "SideDiamondReq");

            migrationBuilder.DeleteData(
                table: "JewelryModelCategory",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "JewelryModelCategory",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.DeleteData(
                table: "JewelryModelCategory",
                keyColumn: "Id",
                keyValue: "3");

            migrationBuilder.DeleteData(
                table: "JewelryModelCategory",
                keyColumn: "Id",
                keyValue: "4");

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
                    { "060a6f41-2a58-4580-bd0e-174cc052c6a6", "THREE_MONTHS", new DateTime(2024, 10, 15, 17, 0, 0, 0, DateTimeKind.Utc), 3, "Default_Diamond_Warranty", 0m, "0" },
                    { "db4e6535-19ac-42f8-bcc7-37ea9befcbd2", "THREE_MONTHS", new DateTime(2024, 10, 15, 17, 0, 0, 0, DateTimeKind.Utc), 3, "Default_Jewelry_Warranty", 0m, "0" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SideDiamondReq_ShapeId",
                table: "SideDiamondReq",
                column: "ShapeId",
                unique: true);
        }
    }
}
