using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDiamondPriceCriteria : Migration
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

            migrationBuilder.DropPrimaryKey(
                name: "PK_DiamondPrice",
                table: "DiamondPrice");

            migrationBuilder.DropIndex(
                name: "IX_DiamondCriteria_ShapeId_IsLabGrown_IsSideDiamond_CaratFrom_~",
                table: "DiamondCriteria");

            migrationBuilder.DropColumn(
                name: "Clarity",
                table: "DiamondCriteria");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "DiamondCriteria");

            migrationBuilder.DropColumn(
                name: "Cut",
                table: "DiamondCriteria");

            migrationBuilder.AlterColumn<string>(
                name: "OrderId",
                table: "Notification",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "Notification",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "DiamondPrice",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "DiamondPrice",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Clarity",
                table: "DiamondPrice",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Color",
                table: "DiamondPrice",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Cut",
                table: "DiamondPrice",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedTable",
                table: "DiamondCriteria",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DiamondPrice",
                table: "DiamondPrice",
                column: "Id");

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
                name: "IX_DiamondPrice_CriteriaId_IsLabDiamond_IsSideDiamond_Cut_Colo~",
                table: "DiamondPrice",
                columns: new[] { "CriteriaId", "IsLabDiamond", "IsSideDiamond", "Cut", "Color", "Clarity" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiamondCriteria_ShapeId_IsSideDiamond_CaratFrom_CaratTo",
                table: "DiamondCriteria",
                columns: new[] { "ShapeId", "IsSideDiamond", "CaratFrom", "CaratTo" });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Account_AccountId",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Order_OrderId",
                table: "Notification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DiamondPrice",
                table: "DiamondPrice");

            migrationBuilder.DropIndex(
                name: "IX_DiamondPrice_CriteriaId_IsLabDiamond_IsSideDiamond_Cut_Colo~",
                table: "DiamondPrice");

            migrationBuilder.DropIndex(
                name: "IX_DiamondCriteria_ShapeId_IsSideDiamond_CaratFrom_CaratTo",
                table: "DiamondCriteria");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "DiamondPrice");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "DiamondPrice");

            migrationBuilder.DropColumn(
                name: "Clarity",
                table: "DiamondPrice");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "DiamondPrice");

            migrationBuilder.DropColumn(
                name: "Cut",
                table: "DiamondPrice");

            migrationBuilder.DropColumn(
                name: "LastUpdatedTable",
                table: "DiamondCriteria");

            migrationBuilder.AlterColumn<string>(
                name: "OrderId",
                table: "Notification",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "Notification",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Clarity",
                table: "DiamondCriteria",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Color",
                table: "DiamondCriteria",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Cut",
                table: "DiamondCriteria",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DiamondPrice",
                table: "DiamondPrice",
                columns: new[] { "CriteriaId", "IsLabDiamond", "IsSideDiamond" });

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

            migrationBuilder.CreateIndex(
                name: "IX_DiamondCriteria_ShapeId_IsLabGrown_IsSideDiamond_CaratFrom_~",
                table: "DiamondCriteria",
                columns: new[] { "ShapeId", "IsLabGrown", "IsSideDiamond", "CaratFrom", "CaratTo" });

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
    }
}
