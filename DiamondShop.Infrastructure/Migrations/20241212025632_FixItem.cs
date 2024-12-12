using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Diamond_DiamondId",
                table: "OrderItem");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Jewelry_JewelryId",
                table: "OrderItem");

            migrationBuilder.DropIndex(
                name: "IX_OrderItem_DiamondId",
                table: "OrderItem");

            migrationBuilder.DropIndex(
                name: "IX_OrderItem_JewelryId",
                table: "OrderItem");

            migrationBuilder.DropIndex(
                name: "IX_DiamondPrice_CriteriaId_IsLabDiamond_IsSideDiamond_Cut_Colo~",
                table: "DiamondPrice");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "OrderItem",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductId",
                table: "OrderItem",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Version",
                table: "Jewelry",
                type: "bytea",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldRowVersion: true);

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
                name: "IX_OrderItem_DiamondId",
                table: "OrderItem",
                column: "DiamondId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_JewelryId",
                table: "OrderItem",
                column: "JewelryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiamondPrice_CriteriaId_IsLabDiamond_IsSideDiamond_Color_Cl~",
                table: "DiamondPrice",
                columns: new[] { "CriteriaId", "IsLabDiamond", "IsSideDiamond", "Color", "Clarity" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Diamond_DiamondId",
                table: "OrderItem",
                column: "DiamondId",
                principalTable: "Diamond",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Jewelry_JewelryId",
                table: "OrderItem",
                column: "JewelryId",
                principalTable: "Jewelry",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Diamond_DiamondId",
                table: "OrderItem");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Jewelry_JewelryId",
                table: "OrderItem");

            migrationBuilder.DropIndex(
                name: "IX_OrderItem_DiamondId",
                table: "OrderItem");

            migrationBuilder.DropIndex(
                name: "IX_OrderItem_JewelryId",
                table: "OrderItem");

            migrationBuilder.DropIndex(
                name: "IX_DiamondPrice_CriteriaId_IsLabDiamond_IsSideDiamond_Color_Cl~",
                table: "DiamondPrice");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "OrderItem");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Version",
                table: "Jewelry",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldRowVersion: true,
                oldNullable: true);

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
                name: "IX_OrderItem_DiamondId",
                table: "OrderItem",
                column: "DiamondId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_JewelryId",
                table: "OrderItem",
                column: "JewelryId");

            migrationBuilder.CreateIndex(
                name: "IX_DiamondPrice_CriteriaId_IsLabDiamond_IsSideDiamond_Cut_Colo~",
                table: "DiamondPrice",
                columns: new[] { "CriteriaId", "IsLabDiamond", "IsSideDiamond", "Cut", "Color", "Clarity" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Diamond_DiamondId",
                table: "OrderItem",
                column: "DiamondId",
                principalTable: "Diamond",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Jewelry_JewelryId",
                table: "OrderItem",
                column: "JewelryId",
                principalTable: "Jewelry",
                principalColumn: "Id");
        }
    }
}
