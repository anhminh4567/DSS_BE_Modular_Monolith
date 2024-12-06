using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsOrderTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "Gift",
                newName: "ItemCode");

            migrationBuilder.AddColumn<string>(
                name: "ShopAccount",
                table: "Transaction",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShopBank",
                table: "Transaction",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxSupportedPrice",
                table: "PaymentMethod",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Order",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ShippingFeeSaved",
                table: "Order",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPromotionAmountSaved",
                table: "Order",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "Jewelry",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<decimal>(
                name: "MoneyLimit",
                table: "Discount",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "Diamond",
                type: "bytea",
                rowVersion: true,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "PaymentMethod",
                keyColumn: "Id",
                keyValue: "1",
                column: "MaxSupportedPrice",
                value: null);

            migrationBuilder.UpdateData(
                table: "PaymentMethod",
                keyColumn: "Id",
                keyValue: "2",
                column: "MaxSupportedPrice",
                value: 50000000m);

            migrationBuilder.UpdateData(
                table: "PaymentMethod",
                keyColumn: "Id",
                keyValue: "3",
                column: "MaxSupportedPrice",
                value: null);

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 12, 6, 4, 31, 3, 132, DateTimeKind.Utc).AddTicks(9129));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 12, 6, 4, 31, 3, 132, DateTimeKind.Utc).AddTicks(9336));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreateDate",
                value: new DateTime(2024, 12, 6, 4, 31, 3, 132, DateTimeKind.Utc).AddTicks(9342));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreateDate",
                value: new DateTime(2024, 12, 6, 4, 31, 3, 132, DateTimeKind.Utc).AddTicks(9344));

            migrationBuilder.CreateIndex(
                name: "IX_JewelryModel_ModelCode",
                table: "JewelryModel",
                column: "ModelCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jewelry_SerialCode",
                table: "Jewelry",
                column: "SerialCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Diamond_SerialCode",
                table: "Diamond",
                column: "SerialCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JewelryModel_ModelCode",
                table: "JewelryModel");

            migrationBuilder.DropIndex(
                name: "IX_Jewelry_SerialCode",
                table: "Jewelry");

            migrationBuilder.DropIndex(
                name: "IX_Diamond_SerialCode",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "ShopAccount",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "ShopBank",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "MaxSupportedPrice",
                table: "PaymentMethod");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ShippingFeeSaved",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "TotalPromotionAmountSaved",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Jewelry");

            migrationBuilder.DropColumn(
                name: "MoneyLimit",
                table: "Discount");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Diamond");

            migrationBuilder.RenameColumn(
                name: "ItemCode",
                table: "Gift",
                newName: "ItemId");

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
    }
}
