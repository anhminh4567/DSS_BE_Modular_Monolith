using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewTransac : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TimeStampe",
                table: "Transaction",
                newName: "TimeStamp");

            migrationBuilder.AlterColumn<string>(
                name: "PaygateTransactionCode",
                table: "Transaction",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "AppTransactionCode",
                table: "Transaction",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Evidence",
                table: "Transaction",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "Transaction",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VerifierId",
                table: "Transaction",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DepositFee",
                table: "Order",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

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

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_VerifierId",
                table: "Transaction",
                column: "VerifierId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Account_VerifierId",
                table: "Transaction",
                column: "VerifierId",
                principalTable: "Account",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Account_VerifierId",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_VerifierId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "Evidence",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "VerifierId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "DepositFee",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "TimeStamp",
                table: "Transaction",
                newName: "TimeStampe");

            migrationBuilder.AlterColumn<string>(
                name: "PaygateTransactionCode",
                table: "Transaction",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AppTransactionCode",
                table: "Transaction",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 11, 27, 3, 58, 59, 761, DateTimeKind.Utc).AddTicks(3957));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 11, 27, 3, 58, 59, 761, DateTimeKind.Utc).AddTicks(4225));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreateDate",
                value: new DateTime(2024, 11, 27, 3, 58, 59, 761, DateTimeKind.Utc).AddTicks(4231));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreateDate",
                value: new DateTime(2024, 11, 27, 3, 58, 59, 761, DateTimeKind.Utc).AddTicks(4232));
        }
    }
}
