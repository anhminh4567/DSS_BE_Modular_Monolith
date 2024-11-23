using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderLogRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderLog_OrderLog_PreviousLogId",
                table: "OrderLog");

            migrationBuilder.DropIndex(
                name: "IX_OrderLog_PreviousLogId",
                table: "OrderLog");

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 11, 23, 9, 29, 21, 740, DateTimeKind.Utc).AddTicks(3586));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 11, 23, 9, 29, 21, 740, DateTimeKind.Utc).AddTicks(3788));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreateDate",
                value: new DateTime(2024, 11, 23, 9, 29, 21, 740, DateTimeKind.Utc).AddTicks(3795));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreateDate",
                value: new DateTime(2024, 11, 23, 9, 29, 21, 740, DateTimeKind.Utc).AddTicks(3797));

            migrationBuilder.CreateIndex(
                name: "IX_OrderLog_PreviousLogId",
                table: "OrderLog",
                column: "PreviousLogId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLog_OrderLog_PreviousLogId",
                table: "OrderLog",
                column: "PreviousLogId",
                principalTable: "OrderLog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderLog_OrderLog_PreviousLogId",
                table: "OrderLog");

            migrationBuilder.DropIndex(
                name: "IX_OrderLog_PreviousLogId",
                table: "OrderLog");

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

            migrationBuilder.CreateIndex(
                name: "IX_OrderLog_PreviousLogId",
                table: "OrderLog",
                column: "PreviousLogId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLog_OrderLog_PreviousLogId",
                table: "OrderLog",
                column: "PreviousLogId",
                principalTable: "OrderLog",
                principalColumn: "Id");
        }
    }
}
