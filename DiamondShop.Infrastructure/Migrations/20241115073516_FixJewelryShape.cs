using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixJewelryShape : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Jewelry_DiamondShapeId",
                table: "Jewelry");

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 11, 15, 7, 35, 14, 355, DateTimeKind.Utc).AddTicks(2608));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 11, 15, 7, 35, 14, 355, DateTimeKind.Utc).AddTicks(2869));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreateDate",
                value: new DateTime(2024, 11, 15, 7, 35, 14, 355, DateTimeKind.Utc).AddTicks(2873));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreateDate",
                value: new DateTime(2024, 11, 15, 7, 35, 14, 355, DateTimeKind.Utc).AddTicks(2874));

            migrationBuilder.CreateIndex(
                name: "IX_Jewelry_DiamondShapeId",
                table: "Jewelry",
                column: "DiamondShapeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Jewelry_DiamondShapeId",
                table: "Jewelry");

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 11, 14, 6, 47, 27, 277, DateTimeKind.Utc).AddTicks(883));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 11, 14, 6, 47, 27, 277, DateTimeKind.Utc).AddTicks(1129));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreateDate",
                value: new DateTime(2024, 11, 14, 6, 47, 27, 277, DateTimeKind.Utc).AddTicks(1134));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreateDate",
                value: new DateTime(2024, 11, 14, 6, 47, 27, 277, DateTimeKind.Utc).AddTicks(1136));

            migrationBuilder.CreateIndex(
                name: "IX_Jewelry_DiamondShapeId",
                table: "Jewelry",
                column: "DiamondShapeId",
                unique: true);
        }
    }
}
