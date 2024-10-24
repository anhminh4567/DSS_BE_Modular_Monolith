using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Account_AccountId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_DeliveryPackage_DeliveryPackageId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderLog_DeliveryPackage_DeliveryPackageId",
                table: "OrderLog");

            migrationBuilder.DropTable(
                name: "DeliveryPackage");

            migrationBuilder.DropIndex(
                name: "IX_OrderLog_DeliveryPackageId",
                table: "OrderLog");

            migrationBuilder.DropColumn(
                name: "DeliveryPackageId",
                table: "OrderLog");

            migrationBuilder.DropColumn(
                name: "EngravedFont",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "EngravedText",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Jewelry");

            migrationBuilder.DropColumn(
                name: "IsSold",
                table: "Jewelry");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "IsSold",
                table: "Diamond");

            migrationBuilder.RenameColumn(
                name: "DeliveryPackageId",
                table: "Order",
                newName: "DelivererId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_DeliveryPackageId",
                table: "Order",
                newName: "IX_Order_DelivererId");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "OrderLog",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "FinalPrice",
                table: "OrderItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "D_Price",
                table: "Jewelry",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EngravedFont",
                table: "Jewelry",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EngravedText",
                table: "Jewelry",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ND_Price",
                table: "Jewelry",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SoldPrice",
                table: "Jewelry",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Jewelry",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsLabDiamond",
                table: "DiamondPrice",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "SoldPrice",
                table: "Diamond",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Diamond",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "PhoneNumber",
                table: "Account",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Account_AccountId",
                table: "Order",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Account_DelivererId",
                table: "Order",
                column: "DelivererId",
                principalTable: "Account",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Account_AccountId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Account_DelivererId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "OrderLog");

            migrationBuilder.DropColumn(
                name: "FinalPrice",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "D_Price",
                table: "Jewelry");

            migrationBuilder.DropColumn(
                name: "EngravedFont",
                table: "Jewelry");

            migrationBuilder.DropColumn(
                name: "EngravedText",
                table: "Jewelry");

            migrationBuilder.DropColumn(
                name: "ND_Price",
                table: "Jewelry");

            migrationBuilder.DropColumn(
                name: "SoldPrice",
                table: "Jewelry");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Jewelry");

            migrationBuilder.DropColumn(
                name: "IsLabDiamond",
                table: "DiamondPrice");

            migrationBuilder.DropColumn(
                name: "SoldPrice",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Account");

            migrationBuilder.RenameColumn(
                name: "DelivererId",
                table: "Order",
                newName: "DeliveryPackageId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_DelivererId",
                table: "Order",
                newName: "IX_Order_DeliveryPackageId");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryPackageId",
                table: "OrderLog",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EngravedFont",
                table: "OrderItem",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EngravedText",
                table: "OrderItem",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Jewelry",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSold",
                table: "Jewelry",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Diamond",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSold",
                table: "Diamond",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "DeliveryPackage",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DelivererId = table.Column<string>(type: "text", nullable: true),
                    CompleteDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeliveryMethod = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryPackage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryPackage_Account_DelivererId",
                        column: x => x.DelivererId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderLog_DeliveryPackageId",
                table: "OrderLog",
                column: "DeliveryPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryPackage_DelivererId",
                table: "DeliveryPackage",
                column: "DelivererId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Account_AccountId",
                table: "Order",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_DeliveryPackage_DeliveryPackageId",
                table: "Order",
                column: "DeliveryPackageId",
                principalTable: "DeliveryPackage",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLog_DeliveryPackage_DeliveryPackageId",
                table: "OrderLog",
                column: "DeliveryPackageId",
                principalTable: "DeliveryPackage",
                principalColumn: "Id");
        }
    }
}
