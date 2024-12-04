using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateItemsOrderFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountPercent",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "PromoType",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "OrderSavedAmount",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "IsRhodiumFinish",
                table: "JewelryModel");

            migrationBuilder.RenameColumn(
                name: "PromoValue",
                table: "OrderItem",
                newName: "PromotionSavedAmount");

            migrationBuilder.AddColumn<int>(
                name: "ApplyLevel",
                table: "Promotion",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MoneyLimit",
                table: "Promotion",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountSavedAmount",
                table: "OrderItem",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasDelivererReturned",
                table: "Order",
                type: "boolean",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 12, 4, 12, 49, 20, 456, DateTimeKind.Utc).AddTicks(6209));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 12, 4, 12, 49, 20, 456, DateTimeKind.Utc).AddTicks(6429));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreateDate",
                value: new DateTime(2024, 12, 4, 12, 49, 20, 456, DateTimeKind.Utc).AddTicks(6494));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreateDate",
                value: new DateTime(2024, 12, 4, 12, 49, 20, 456, DateTimeKind.Utc).AddTicks(6498));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplyLevel",
                table: "Promotion");

            migrationBuilder.DropColumn(
                name: "MoneyLimit",
                table: "Promotion");

            migrationBuilder.DropColumn(
                name: "DiscountSavedAmount",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "HasDelivererReturned",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "PromotionSavedAmount",
                table: "OrderItem",
                newName: "PromoValue");

            migrationBuilder.AddColumn<int>(
                name: "DiscountPercent",
                table: "OrderItem",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PromoType",
                table: "OrderItem",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OrderSavedAmount",
                table: "Order",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsRhodiumFinish",
                table: "JewelryModel",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 12, 3, 8, 26, 59, 568, DateTimeKind.Utc).AddTicks(4665));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 12, 3, 8, 26, 59, 568, DateTimeKind.Utc).AddTicks(5117));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreateDate",
                value: new DateTime(2024, 12, 3, 8, 26, 59, 568, DateTimeKind.Utc).AddTicks(5123));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreateDate",
                value: new DateTime(2024, 12, 3, 8, 26, 59, 568, DateTimeKind.Utc).AddTicks(5125));
        }
    }
}
