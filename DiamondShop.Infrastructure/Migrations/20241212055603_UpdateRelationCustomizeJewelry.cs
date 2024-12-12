using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelationCustomizeJewelry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomizeRequest_Jewelry_JewelryId",
                table: "CustomizeRequest");

            migrationBuilder.DropIndex(
                name: "IX_CustomizeRequest_JewelryId",
                table: "CustomizeRequest");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Account",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 12, 12, 5, 56, 2, 81, DateTimeKind.Utc).AddTicks(1471));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 12, 12, 5, 56, 2, 81, DateTimeKind.Utc).AddTicks(1680));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreateDate",
                value: new DateTime(2024, 12, 12, 5, 56, 2, 81, DateTimeKind.Utc).AddTicks(1686));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreateDate",
                value: new DateTime(2024, 12, 12, 5, 56, 2, 81, DateTimeKind.Utc).AddTicks(1688));

            migrationBuilder.CreateIndex(
                name: "IX_CustomizeRequest_JewelryId",
                table: "CustomizeRequest",
                column: "JewelryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Address_ProvinceId",
                table: "Address",
                column: "ProvinceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_AppCities_ProvinceId",
                table: "Address",
                column: "ProvinceId",
                principalTable: "AppCities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomizeRequest_Jewelry_JewelryId",
                table: "CustomizeRequest",
                column: "JewelryId",
                principalTable: "Jewelry",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_AppCities_ProvinceId",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomizeRequest_Jewelry_JewelryId",
                table: "CustomizeRequest");

            migrationBuilder.DropIndex(
                name: "IX_CustomizeRequest_JewelryId",
                table: "CustomizeRequest");

            migrationBuilder.DropIndex(
                name: "IX_Address_ProvinceId",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Account");

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 12, 12, 2, 56, 30, 716, DateTimeKind.Utc).AddTicks(454));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 12, 12, 2, 56, 30, 716, DateTimeKind.Utc).AddTicks(690));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreateDate",
                value: new DateTime(2024, 12, 12, 2, 56, 30, 716, DateTimeKind.Utc).AddTicks(695));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreateDate",
                value: new DateTime(2024, 12, 12, 2, 56, 30, 716, DateTimeKind.Utc).AddTicks(697));

            migrationBuilder.CreateIndex(
                name: "IX_CustomizeRequest_JewelryId",
                table: "CustomizeRequest",
                column: "JewelryId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomizeRequest_Jewelry_JewelryId",
                table: "CustomizeRequest",
                column: "JewelryId",
                principalTable: "Jewelry",
                principalColumn: "Id");
        }
    }
}
