using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNotificationRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Account_AccountId",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Order_OrderId",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_AccountId",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_OrderId",
                table: "Notification");

            migrationBuilder.AddColumn<string>(
                name: "OldAccountIdUpdate",
                table: "DiamondPrice",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OldPrice",
                table: "DiamondPrice",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OldUpdatedAt",
                table: "DiamondPrice",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FixPriceOffset",
                table: "Diamond",
                type: "numeric",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 12, 2, 7, 59, 30, 728, DateTimeKind.Utc).AddTicks(540));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 12, 2, 7, 59, 30, 728, DateTimeKind.Utc).AddTicks(751));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreateDate",
                value: new DateTime(2024, 12, 2, 7, 59, 30, 728, DateTimeKind.Utc).AddTicks(758));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreateDate",
                value: new DateTime(2024, 12, 2, 7, 59, 30, 728, DateTimeKind.Utc).AddTicks(760));

            migrationBuilder.CreateIndex(
                name: "IX_Notification_AccountId",
                table: "Notification",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_OrderId",
                table: "Notification",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Account_AccountId",
                table: "Notification",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Order_OrderId",
                table: "Notification",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Account_AccountId",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Order_OrderId",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_AccountId",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_OrderId",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "OldAccountIdUpdate",
                table: "DiamondPrice");

            migrationBuilder.DropColumn(
                name: "OldPrice",
                table: "DiamondPrice");

            migrationBuilder.DropColumn(
                name: "OldUpdatedAt",
                table: "DiamondPrice");

            migrationBuilder.DropColumn(
                name: "FixPriceOffset",
                table: "Diamond");

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 12, 2, 3, 40, 33, 30, DateTimeKind.Utc).AddTicks(9253));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 12, 2, 3, 40, 33, 30, DateTimeKind.Utc).AddTicks(9499));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreateDate",
                value: new DateTime(2024, 12, 2, 3, 40, 33, 30, DateTimeKind.Utc).AddTicks(9508));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreateDate",
                value: new DateTime(2024, 12, 2, 3, 40, 33, 30, DateTimeKind.Utc).AddTicks(9511));

            migrationBuilder.CreateIndex(
                name: "IX_Notification_AccountId",
                table: "Notification",
                column: "AccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notification_OrderId",
                table: "Notification",
                column: "OrderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Account_AccountId",
                table: "Notification",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Order_OrderId",
                table: "Notification",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id");
        }
    }
}
