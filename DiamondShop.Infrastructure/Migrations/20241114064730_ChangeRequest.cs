using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Clarity",
                table: "DiamondRequest");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "DiamondRequest");

            migrationBuilder.DropColumn(
                name: "Cut",
                table: "DiamondRequest");

            migrationBuilder.AlterColumn<float>(
                name: "CaratTo",
                table: "DiamondRequest",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "CaratFrom",
                table: "DiamondRequest",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClarityFrom",
                table: "DiamondRequest",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClarityTo",
                table: "DiamondRequest",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ColorFrom",
                table: "DiamondRequest",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ColorTo",
                table: "DiamondRequest",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CutFrom",
                table: "DiamondRequest",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CutTo",
                table: "DiamondRequest",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 11, 14, 6, 47, 27, 277, DateTimeKind.Utc).AddTicks(883));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 11, 14, 6, 47, 27, 277, DateTimeKind.Utc).AddTicks(1129));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreateDate",
                value: new DateTime(2024, 11, 14, 6, 47, 27, 277, DateTimeKind.Utc).AddTicks(1134));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreateDate",
                value: new DateTime(2024, 11, 14, 6, 47, 27, 277, DateTimeKind.Utc).AddTicks(1136));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClarityFrom",
                table: "DiamondRequest");

            migrationBuilder.DropColumn(
                name: "ClarityTo",
                table: "DiamondRequest");

            migrationBuilder.DropColumn(
                name: "ColorFrom",
                table: "DiamondRequest");

            migrationBuilder.DropColumn(
                name: "ColorTo",
                table: "DiamondRequest");

            migrationBuilder.DropColumn(
                name: "CutFrom",
                table: "DiamondRequest");

            migrationBuilder.DropColumn(
                name: "CutTo",
                table: "DiamondRequest");

            migrationBuilder.AlterColumn<float>(
                name: "CaratTo",
                table: "DiamondRequest",
                type: "real",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<float>(
                name: "CaratFrom",
                table: "DiamondRequest",
                type: "real",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AddColumn<int>(
                name: "Clarity",
                table: "DiamondRequest",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Color",
                table: "DiamondRequest",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Cut",
                table: "DiamondRequest",
                type: "integer",
                nullable: true);

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
    }
}
