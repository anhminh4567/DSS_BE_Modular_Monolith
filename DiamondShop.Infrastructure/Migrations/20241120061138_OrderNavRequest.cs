using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OrderNavRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Order_ParentOrderId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_ParentOrderId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ParentOrderId",
                table: "Order");

            migrationBuilder.AddColumn<int>(
                name: "Stage",
                table: "CustomizeRequest",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 11, 20, 6, 11, 36, 811, DateTimeKind.Utc).AddTicks(8945));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 11, 20, 6, 11, 36, 811, DateTimeKind.Utc).AddTicks(9223));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreateDate",
                value: new DateTime(2024, 11, 20, 6, 11, 36, 811, DateTimeKind.Utc).AddTicks(9228));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreateDate",
                value: new DateTime(2024, 11, 20, 6, 11, 36, 811, DateTimeKind.Utc).AddTicks(9229));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stage",
                table: "CustomizeRequest");

            migrationBuilder.AddColumn<string>(
                name: "ParentOrderId",
                table: "Order",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 11, 18, 14, 11, 47, 456, DateTimeKind.Utc).AddTicks(3334));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 11, 18, 14, 11, 47, 456, DateTimeKind.Utc).AddTicks(3527));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreateDate",
                value: new DateTime(2024, 11, 18, 14, 11, 47, 456, DateTimeKind.Utc).AddTicks(3532));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreateDate",
                value: new DateTime(2024, 11, 18, 14, 11, 47, 456, DateTimeKind.Utc).AddTicks(3534));

            migrationBuilder.CreateIndex(
                name: "IX_Order_ParentOrderId",
                table: "Order",
                column: "ParentOrderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Order_ParentOrderId",
                table: "Order",
                column: "ParentOrderId",
                principalTable: "Order",
                principalColumn: "Id");
        }
    }
}
