using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateItemsAndRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderItemWarranty_OrderItemId",
                table: "OrderItemWarranty");

            migrationBuilder.RenameColumn(
                name: "PriceOffset",
                table: "JewelryModel",
                newName: "CraftmanFee");

            migrationBuilder.AddColumn<string>(
                name: "WarrantyId",
                table: "OrderItem",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiredDate",
                table: "Order",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Order",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShipFailedCount",
                table: "Order",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ShipFailedDate",
                table: "Order",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SerialCode",
                table: "Diamond",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 10, 20, 17, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 10, 20, 17, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemWarranty_OrderItemId",
                table: "OrderItemWarranty",
                column: "OrderItemId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderItemWarranty_OrderItemId",
                table: "OrderItemWarranty");

            migrationBuilder.DropColumn(
                name: "WarrantyId",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "ExpiredDate",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ShipFailedCount",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ShipFailedDate",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "SerialCode",
                table: "Diamond");

            migrationBuilder.RenameColumn(
                name: "CraftmanFee",
                table: "JewelryModel",
                newName: "PriceOffset");

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 10, 16, 17, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 10, 16, 17, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemWarranty_OrderItemId",
                table: "OrderItemWarranty",
                column: "OrderItemId");
        }
    }
}
