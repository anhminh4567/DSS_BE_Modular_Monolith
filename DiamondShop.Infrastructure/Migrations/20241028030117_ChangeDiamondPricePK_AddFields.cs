using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDiamondPricePK_AddFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DiamondPrice",
                table: "DiamondPrice");

            migrationBuilder.DropIndex(
                name: "IX_DiamondPrice_CriteriaId",
                table: "DiamondPrice");

            migrationBuilder.AddColumn<bool>(
                name: "IsSideDiamond",
                table: "DiamondPrice",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSideDiamond",
                table: "DiamondCriteria",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DiamondPrice",
                table: "DiamondPrice",
                columns: new[] { "ShapeId", "CriteriaId", "IsLabDiamond", "IsSideDiamond" });

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 10, 27, 17, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 10, 27, 17, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.CreateIndex(
                name: "IX_DiamondPrice_CriteriaId_IsLabDiamond_IsSideDiamond",
                table: "DiamondPrice",
                columns: new[] { "CriteriaId", "IsLabDiamond", "IsSideDiamond" });

            migrationBuilder.CreateIndex(
                name: "IX_DiamondPrice_CriteriaId_ShapeId_IsLabDiamond_IsSideDiamond",
                table: "DiamondPrice",
                columns: new[] { "CriteriaId", "ShapeId", "IsLabDiamond", "IsSideDiamond" });

            migrationBuilder.CreateIndex(
                name: "IX_DiamondPrice_ShapeId_IsLabDiamond_IsSideDiamond",
                table: "DiamondPrice",
                columns: new[] { "ShapeId", "IsLabDiamond", "IsSideDiamond" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DiamondPrice",
                table: "DiamondPrice");

            migrationBuilder.DropIndex(
                name: "IX_DiamondPrice_CriteriaId_IsLabDiamond_IsSideDiamond",
                table: "DiamondPrice");

            migrationBuilder.DropIndex(
                name: "IX_DiamondPrice_CriteriaId_ShapeId_IsLabDiamond_IsSideDiamond",
                table: "DiamondPrice");

            migrationBuilder.DropIndex(
                name: "IX_DiamondPrice_ShapeId_IsLabDiamond_IsSideDiamond",
                table: "DiamondPrice");

            migrationBuilder.DropColumn(
                name: "IsSideDiamond",
                table: "DiamondPrice");

            migrationBuilder.DropColumn(
                name: "IsSideDiamond",
                table: "DiamondCriteria");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DiamondPrice",
                table: "DiamondPrice",
                columns: new[] { "ShapeId", "CriteriaId" });

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 10, 24, 17, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 10, 24, 17, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.CreateIndex(
                name: "IX_DiamondPrice_CriteriaId",
                table: "DiamondPrice",
                column: "CriteriaId");
        }
    }
}
