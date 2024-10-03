using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeliveryAndMedias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TransactionCode",
                table: "Transaction",
                newName: "TimeStampe");

            migrationBuilder.RenameColumn(
                name: "Note",
                table: "Transaction",
                newName: "PaygateTransactionCode");

            migrationBuilder.AddColumn<string>(
                name: "AppTransactionCode",
                table: "Transaction",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Transaction",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "FineAmount",
                table: "Transaction",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Thumbnail",
                table: "Promotion",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryPackageId",
                table: "OrderLog",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogImages",
                table: "OrderLog",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryPackageId",
                table: "Order",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentOrderId",
                table: "Order",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Images",
                table: "JewelryReview",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Thumbnail",
                table: "JewelryModel",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Thumbnail",
                table: "Jewelry",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Thumbnail",
                table: "Discount",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Thumbnail",
                table: "Diamond",
                type: "jsonb",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DeliveryFee",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DeliveryMethod = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Cost = table.Column<decimal>(type: "numeric", nullable: false),
                    FromKm = table.Column<int>(type: "integer", nullable: false),
                    ToKm = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryFee", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryPackage",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompleteDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DeliveryMethod = table.Column<string>(type: "text", nullable: true),
                    DelivererId = table.Column<string>(type: "text", nullable: true)
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

            migrationBuilder.InsertData(
                table: "Account_Role",
                columns: new[] { "Id", "RoleDescription", "RoleName", "RoleType" },
                values: new object[] { "44", "deliverer", "deliverer", 1 });

            migrationBuilder.CreateIndex(
                name: "IX_OrderLog_DeliveryPackageId",
                table: "OrderLog",
                column: "DeliveryPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_DeliveryPackageId",
                table: "Order",
                column: "DeliveryPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_ParentOrderId",
                table: "Order",
                column: "ParentOrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryPackage_DelivererId",
                table: "DeliveryPackage",
                column: "DelivererId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_DeliveryPackage_DeliveryPackageId",
                table: "Order",
                column: "DeliveryPackageId",
                principalTable: "DeliveryPackage",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Order_ParentOrderId",
                table: "Order",
                column: "ParentOrderId",
                principalTable: "Order",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLog_DeliveryPackage_DeliveryPackageId",
                table: "OrderLog",
                column: "DeliveryPackageId",
                principalTable: "DeliveryPackage",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_DeliveryPackage_DeliveryPackageId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Order_ParentOrderId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderLog_DeliveryPackage_DeliveryPackageId",
                table: "OrderLog");

            migrationBuilder.DropTable(
                name: "DeliveryFee");

            migrationBuilder.DropTable(
                name: "DeliveryPackage");

            migrationBuilder.DropIndex(
                name: "IX_OrderLog_DeliveryPackageId",
                table: "OrderLog");

            migrationBuilder.DropIndex(
                name: "IX_Order_DeliveryPackageId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_ParentOrderId",
                table: "Order");

            migrationBuilder.DeleteData(
                table: "Account_Role",
                keyColumn: "Id",
                keyValue: "44");

            migrationBuilder.DropColumn(
                name: "AppTransactionCode",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "FineAmount",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "Thumbnail",
                table: "Promotion");

            migrationBuilder.DropColumn(
                name: "DeliveryPackageId",
                table: "OrderLog");

            migrationBuilder.DropColumn(
                name: "LogImages",
                table: "OrderLog");

            migrationBuilder.DropColumn(
                name: "DeliveryPackageId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ParentOrderId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Images",
                table: "JewelryReview");

            migrationBuilder.DropColumn(
                name: "Thumbnail",
                table: "JewelryModel");

            migrationBuilder.DropColumn(
                name: "Thumbnail",
                table: "Jewelry");

            migrationBuilder.DropColumn(
                name: "Thumbnail",
                table: "Discount");

            migrationBuilder.DropColumn(
                name: "Thumbnail",
                table: "Diamond");

            migrationBuilder.RenameColumn(
                name: "TimeStampe",
                table: "Transaction",
                newName: "TransactionCode");

            migrationBuilder.RenameColumn(
                name: "PaygateTransactionCode",
                table: "Transaction",
                newName: "Note");
        }
    }
}
