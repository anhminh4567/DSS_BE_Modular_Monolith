using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMissingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PayDate",
                table: "Transaction",
                newName: "VerifiedDate");

            migrationBuilder.RenameColumn(
                name: "IsVerified",
                table: "Transaction",
                newName: "IsLegit");

            migrationBuilder.AddColumn<DateTime>(
                name: "InitDate",
                table: "Transaction",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Transaction",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Amount",
                table: "Promotion",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Notification",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxAmout",
                table: "Gift",
                type: "numeric",
                nullable: true);

            migrationBuilder.InsertData(
                table: "PaymentMethod",
                columns: new[] { "Id", "MethodName", "MethodThumbnailPath", "Status" },
                values: new object[] { "3", "CASH", null, true });

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 11, 30, 8, 26, 51, 414, DateTimeKind.Utc).AddTicks(1945));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 11, 30, 8, 26, 51, 414, DateTimeKind.Utc).AddTicks(2159));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreateDate",
                value: new DateTime(2024, 11, 30, 8, 26, 51, 414, DateTimeKind.Utc).AddTicks(2166));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreateDate",
                value: new DateTime(2024, 11, 30, 8, 26, 51, 414, DateTimeKind.Utc).AddTicks(2168));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PaymentMethod",
                keyColumn: "Id",
                keyValue: "3");

            migrationBuilder.DropColumn(
                name: "InitDate",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Promotion");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "MaxAmout",
                table: "Gift");

            migrationBuilder.RenameColumn(
                name: "VerifiedDate",
                table: "Transaction",
                newName: "PayDate");

            migrationBuilder.RenameColumn(
                name: "IsLegit",
                table: "Transaction",
                newName: "IsVerified");

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 11, 27, 18, 32, 44, 381, DateTimeKind.Utc).AddTicks(145));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 11, 27, 18, 32, 44, 381, DateTimeKind.Utc).AddTicks(422));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreateDate",
                value: new DateTime(2024, 11, 27, 18, 32, 44, 381, DateTimeKind.Utc).AddTicks(427));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreateDate",
                value: new DateTime(2024, 11, 27, 18, 32, 44, 381, DateTimeKind.Utc).AddTicks(429));
        }
    }
}
