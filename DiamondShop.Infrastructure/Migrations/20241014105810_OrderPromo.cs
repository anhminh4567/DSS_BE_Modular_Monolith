using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OrderPromo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderItemWarranty",
                table: "OrderItemWarranty");

            migrationBuilder.DropColumn(
                name: "DiscountCode",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "PromoCode",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "PromoPercent",
                table: "OrderItem");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "OrderItemWarranty",
                newName: "Id");

            migrationBuilder.AddColumn<int>(
                name: "MonthDuration",
                table: "Warranty",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PromoCode",
                table: "Promotion",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "EngravedText",
                table: "OrderItem",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "EngravedFont",
                table: "OrderItem",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "DiscountPercent",
                table: "OrderItem",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "DiscountId",
                table: "OrderItem",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PromoType",
                table: "OrderItem",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PromoValue",
                table: "OrderItem",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PromotionId",
                table: "Order",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderItemWarranty",
                table: "OrderItemWarranty",
                column: "Id");

            migrationBuilder.InsertData(
                table: "Warranty",
                columns: new[] { "Id", "Code", "CreateDate", "MonthDuration", "Name", "Price", "Type" },
                values: new object[,]
                {
                    { "4548e415-52cb-406a-bf13-65c60f644399", "THREE_MONTHS", new DateTime(2024, 10, 13, 17, 0, 0, 0, DateTimeKind.Utc), 3, "Default_Jewelry_Warranty", 0m, "0" },
                    { "80a27926-df31-4c31-a836-0005fb7c977b", "THREE_MONTHS", new DateTime(2024, 10, 13, 17, 0, 0, 0, DateTimeKind.Utc), 3, "Default_Diamond_Warranty", 0m, "0" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemWarranty_OrderItemId",
                table: "OrderItemWarranty",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_DiscountId",
                table: "OrderItem",
                column: "DiscountId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_PromotionId",
                table: "Order",
                column: "PromotionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Promotion_PromotionId",
                table: "Order",
                column: "PromotionId",
                principalTable: "Promotion",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Discount_DiscountId",
                table: "OrderItem",
                column: "DiscountId",
                principalTable: "Discount",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Promotion_PromotionId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Discount_DiscountId",
                table: "OrderItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderItemWarranty",
                table: "OrderItemWarranty");

            migrationBuilder.DropIndex(
                name: "IX_OrderItemWarranty_OrderItemId",
                table: "OrderItemWarranty");

            migrationBuilder.DropIndex(
                name: "IX_OrderItem_DiscountId",
                table: "OrderItem");

            migrationBuilder.DropIndex(
                name: "IX_Order_PromotionId",
                table: "Order");

            migrationBuilder.DeleteData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4548e415-52cb-406a-bf13-65c60f644399");

            migrationBuilder.DeleteData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "80a27926-df31-4c31-a836-0005fb7c977b");

            migrationBuilder.DropColumn(
                name: "MonthDuration",
                table: "Warranty");

            migrationBuilder.DropColumn(
                name: "PromoCode",
                table: "Promotion");

            migrationBuilder.DropColumn(
                name: "DiscountId",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "PromoType",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "PromoValue",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "PromotionId",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "OrderItemWarranty",
                newName: "ItemId");

            migrationBuilder.AlterColumn<string>(
                name: "EngravedText",
                table: "OrderItem",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EngravedFont",
                table: "OrderItem",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DiscountPercent",
                table: "OrderItem",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiscountCode",
                table: "OrderItem",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PromoCode",
                table: "OrderItem",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PromoPercent",
                table: "OrderItem",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderItemWarranty",
                table: "OrderItemWarranty",
                columns: new[] { "OrderItemId", "ItemId" });
        }
    }
}
