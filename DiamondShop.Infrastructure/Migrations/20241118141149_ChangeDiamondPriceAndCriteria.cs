using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDiamondPriceAndCriteria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiamondPrice_Diamond_Shape_ShapeId",
                table: "DiamondPrice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DiamondPrice",
                table: "DiamondPrice");

            migrationBuilder.DropIndex(
                name: "IX_DiamondPrice_CriteriaId_ShapeId_IsLabDiamond_IsSideDiamond",
                table: "DiamondPrice");

            migrationBuilder.DropIndex(
                name: "IX_DiamondPrice_ShapeId_IsLabDiamond_IsSideDiamond",
                table: "DiamondPrice");

            migrationBuilder.DropIndex(
                name: "IX_DiamondCriteria_CaratFrom_CaratTo_IsSideDiamond_IsLabGrown",
                table: "DiamondCriteria");

            migrationBuilder.DropColumn(
                name: "ShapeId",
                table: "DiamondPrice");

            migrationBuilder.AddColumn<string>(
                name: "DiscountCode",
                table: "OrderItem",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PromotionCode",
                table: "Order",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "DiamondPrice",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ShapeId",
                table: "DiamondCriteria",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Diamond",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Diamond",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_DiamondPrice",
                table: "DiamondPrice",
                columns: new[] { "CriteriaId", "IsLabDiamond", "IsSideDiamond" });

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
                name: "IX_DiamondPrice_IsLabDiamond_IsSideDiamond",
                table: "DiamondPrice",
                columns: new[] { "IsLabDiamond", "IsSideDiamond" });

            migrationBuilder.CreateIndex(
                name: "IX_DiamondCriteria_CaratFrom_CaratTo_IsSideDiamond",
                table: "DiamondCriteria",
                columns: new[] { "CaratFrom", "CaratTo", "IsSideDiamond" });

            migrationBuilder.CreateIndex(
                name: "IX_DiamondCriteria_ShapeId_IsLabGrown_IsSideDiamond_CaratFrom_~",
                table: "DiamondCriteria",
                columns: new[] { "ShapeId", "IsLabGrown", "IsSideDiamond", "CaratFrom", "CaratTo" });

            migrationBuilder.AddForeignKey(
                name: "FK_DiamondCriteria_Diamond_Shape_ShapeId",
                table: "DiamondCriteria",
                column: "ShapeId",
                principalTable: "Diamond_Shape",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiamondCriteria_Diamond_Shape_ShapeId",
                table: "DiamondCriteria");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DiamondPrice",
                table: "DiamondPrice");

            migrationBuilder.DropIndex(
                name: "IX_DiamondPrice_IsLabDiamond_IsSideDiamond",
                table: "DiamondPrice");

            migrationBuilder.DropIndex(
                name: "IX_DiamondCriteria_CaratFrom_CaratTo_IsSideDiamond",
                table: "DiamondCriteria");

            migrationBuilder.DropIndex(
                name: "IX_DiamondCriteria_ShapeId_IsLabGrown_IsSideDiamond_CaratFrom_~",
                table: "DiamondCriteria");

            migrationBuilder.DropColumn(
                name: "DiscountCode",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "PromotionCode",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "DiamondPrice");

            migrationBuilder.DropColumn(
                name: "ShapeId",
                table: "DiamondCriteria");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Diamond");

            migrationBuilder.AddColumn<string>(
                name: "ShapeId",
                table: "DiamondPrice",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DiamondPrice",
                table: "DiamondPrice",
                columns: new[] { "ShapeId", "CriteriaId", "IsLabDiamond", "IsSideDiamond" });

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 11, 15, 11, 1, 40, 568, DateTimeKind.Utc).AddTicks(9032));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 11, 15, 11, 1, 40, 568, DateTimeKind.Utc).AddTicks(9250));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreateDate",
                value: new DateTime(2024, 11, 15, 11, 1, 40, 568, DateTimeKind.Utc).AddTicks(9255));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreateDate",
                value: new DateTime(2024, 11, 15, 11, 1, 40, 568, DateTimeKind.Utc).AddTicks(9257));

            migrationBuilder.CreateIndex(
                name: "IX_DiamondPrice_CriteriaId_ShapeId_IsLabDiamond_IsSideDiamond",
                table: "DiamondPrice",
                columns: new[] { "CriteriaId", "ShapeId", "IsLabDiamond", "IsSideDiamond" });

            migrationBuilder.CreateIndex(
                name: "IX_DiamondPrice_ShapeId_IsLabDiamond_IsSideDiamond",
                table: "DiamondPrice",
                columns: new[] { "ShapeId", "IsLabDiamond", "IsSideDiamond" });

            migrationBuilder.CreateIndex(
                name: "IX_DiamondCriteria_CaratFrom_CaratTo_IsSideDiamond_IsLabGrown",
                table: "DiamondCriteria",
                columns: new[] { "CaratFrom", "CaratTo", "IsSideDiamond", "IsLabGrown" });

            migrationBuilder.AddForeignKey(
                name: "FK_DiamondPrice_Diamond_Shape_ShapeId",
                table: "DiamondPrice",
                column: "ShapeId",
                principalTable: "Diamond_Shape",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
