using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RequestCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "OrderAmountSaved",
                table: "Order",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UserRankAmountSaved",
                table: "Order",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "RequestCode",
                table: "CustomizeRequest",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 11, 13, 8, 8, 34, 653, DateTimeKind.Utc).AddTicks(8939));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 11, 13, 8, 8, 34, 653, DateTimeKind.Utc).AddTicks(9200));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreateDate",
                value: new DateTime(2024, 11, 13, 8, 8, 34, 653, DateTimeKind.Utc).AddTicks(9205));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreateDate",
                value: new DateTime(2024, 11, 13, 8, 8, 34, 653, DateTimeKind.Utc).AddTicks(9207));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderAmountSaved",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "UserRankAmountSaved",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "RequestCode",
                table: "CustomizeRequest");

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 11, 11, 17, 57, 42, 412, DateTimeKind.Utc).AddTicks(35));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 11, 11, 17, 57, 42, 412, DateTimeKind.Utc).AddTicks(299));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreateDate",
                value: new DateTime(2024, 11, 11, 17, 57, 42, 412, DateTimeKind.Utc).AddTicks(304));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreateDate",
                value: new DateTime(2024, 11, 11, 17, 57, 42, 412, DateTimeKind.Utc).AddTicks(306));
        }
    }
}
