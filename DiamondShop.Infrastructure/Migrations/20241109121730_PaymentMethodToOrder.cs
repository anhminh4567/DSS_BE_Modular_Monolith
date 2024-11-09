using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PaymentMethodToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentMethodId",
                table: "Order",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "PaymentMethod",
                keyColumn: "Id",
                keyValue: "1",
                column: "MethodName",
                value: "BANK_TRANSFER");

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 11, 9, 12, 17, 28, 88, DateTimeKind.Utc).AddTicks(2160));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 11, 9, 12, 17, 28, 88, DateTimeKind.Utc).AddTicks(2390));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreateDate",
                value: new DateTime(2024, 11, 9, 12, 17, 28, 88, DateTimeKind.Utc).AddTicks(2395));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreateDate",
                value: new DateTime(2024, 11, 9, 12, 17, 28, 88, DateTimeKind.Utc).AddTicks(2398));

            migrationBuilder.CreateIndex(
                name: "IX_Order_PaymentMethodId",
                table: "Order",
                column: "PaymentMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_PaymentMethod_PaymentMethodId",
                table: "Order",
                column: "PaymentMethodId",
                principalTable: "PaymentMethod",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_PaymentMethod_PaymentMethodId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_PaymentMethodId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "PaymentMethodId",
                table: "Order");

            migrationBuilder.UpdateData(
                table: "PaymentMethod",
                keyColumn: "Id",
                keyValue: "1",
                column: "MethodName",
                value: "COD");

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 11, 8, 11, 11, 14, 145, DateTimeKind.Utc).AddTicks(6826));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 11, 8, 11, 11, 14, 145, DateTimeKind.Utc).AddTicks(7506));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreateDate",
                value: new DateTime(2024, 11, 8, 11, 11, 14, 145, DateTimeKind.Utc).AddTicks(7513));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreateDate",
                value: new DateTime(2024, 11, 8, 11, 11, 14, 145, DateTimeKind.Utc).AddTicks(7515));
        }
    }
}
