using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixOrderItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderItem_DiamondId",
                table: "OrderItem");

            migrationBuilder.DropIndex(
                name: "IX_OrderItem_JewelryId",
                table: "OrderItem");

            migrationBuilder.DeleteData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4548e415-52cb-406a-bf13-65c60f644399");

            migrationBuilder.DeleteData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "80a27926-df31-4c31-a836-0005fb7c977b");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CompleteDate",
                table: "DeliveryPackage",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.InsertData(
                table: "Warranty",
                columns: new[] { "Id", "Code", "CreateDate", "MonthDuration", "Name", "Price", "Type" },
                values: new object[,]
                {
                    { "35be9da2-258c-44f8-b2c4-ccbd55fc636a", "THREE_MONTHS", new DateTime(2024, 10, 14, 17, 0, 0, 0, DateTimeKind.Utc), 3, "Default_Diamond_Warranty", 0m, "0" },
                    { "9a041044-4385-40fb-a0dc-e2f604b70584", "THREE_MONTHS", new DateTime(2024, 10, 14, 17, 0, 0, 0, DateTimeKind.Utc), 3, "Default_Jewelry_Warranty", 0m, "0" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_DiamondId",
                table: "OrderItem",
                column: "DiamondId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_JewelryId",
                table: "OrderItem",
                column: "JewelryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderItem_DiamondId",
                table: "OrderItem");

            migrationBuilder.DropIndex(
                name: "IX_OrderItem_JewelryId",
                table: "OrderItem");

            migrationBuilder.DeleteData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "35be9da2-258c-44f8-b2c4-ccbd55fc636a");

            migrationBuilder.DeleteData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "9a041044-4385-40fb-a0dc-e2f604b70584");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CompleteDate",
                table: "DeliveryPackage",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Warranty",
                columns: new[] { "Id", "Code", "CreateDate", "MonthDuration", "Name", "Price", "Type" },
                values: new object[,]
                {
                    { "4548e415-52cb-406a-bf13-65c60f644399", "THREE_MONTHS", new DateTime(2024, 10, 13, 17, 0, 0, 0, DateTimeKind.Utc), 3, "Default_Jewelry_Warranty", 0m, "0" },
                    { "80a27926-df31-4c31-a836-0005fb7c977b", "THREE_MONTHS", new DateTime(2024, 10, 13, 17, 0, 0, 0, DateTimeKind.Utc), 3, "Default_Diamond_Warranty", 0m, "0" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_DiamondId",
                table: "OrderItem",
                column: "DiamondId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_JewelryId",
                table: "OrderItem",
                column: "JewelryId",
                unique: true);
        }
    }
}
