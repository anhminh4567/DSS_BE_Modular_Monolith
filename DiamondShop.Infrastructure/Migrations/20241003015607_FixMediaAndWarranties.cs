using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixMediaAndWarranties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JewelryReview_Account_AccountId1",
                table: "JewelryReview");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Account_AccountId1",
                table: "Order");

            migrationBuilder.DropTable(
                name: "DiamondWarranty");

            migrationBuilder.DropTable(
                name: "JewelryWarranty");

            migrationBuilder.DropIndex(
                name: "IX_Order_AccountId1",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_JewelryReview_AccountId1",
                table: "JewelryReview");

            migrationBuilder.DropColumn(
                name: "AccountId1",
                table: "JewelryReview");

            migrationBuilder.RenameColumn(
                name: "AccountId1",
                table: "Order",
                newName: "CustomizeRequestId");

            migrationBuilder.AddColumn<string>(
                name: "Thumbnail",
                table: "Metal",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gallery",
                table: "JewelryModel",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLabGrown",
                table: "DiamondCriteria",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Gallery",
                table: "Diamond",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Thumbnail",
                table: "Blog",
                type: "jsonb",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomizeRequest",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    AccountId = table.Column<string>(type: "text", nullable: false),
                    JewelryModelId = table.Column<string>(type: "text", nullable: false),
                    SizeId = table.Column<string>(type: "text", nullable: false),
                    MetalId = table.Column<string>(type: "text", nullable: false),
                    EngravedText = table.Column<string>(type: "text", nullable: true),
                    EngravedFont = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomizeRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomizeRequest_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomizeRequest_JewelryModel_JewelryModelId",
                        column: x => x.JewelryModelId,
                        principalTable: "JewelryModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomizeRequest_Metal_MetalId",
                        column: x => x.MetalId,
                        principalTable: "Metal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomizeRequest_Size_SizeId",
                        column: x => x.SizeId,
                        principalTable: "Size",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemWarranty",
                columns: table => new
                {
                    OrderItemId = table.Column<string>(type: "text", nullable: false),
                    ItemId = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiredDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    WarrantyType = table.Column<string>(type: "text", nullable: false),
                    WarrantyCode = table.Column<string>(type: "text", nullable: false),
                    WarrantyPath = table.Column<string>(type: "text", nullable: false),
                    SoldPrice = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemWarranty", x => new { x.OrderItemId, x.ItemId });
                    table.ForeignKey(
                        name: "FK_OrderItemWarranty_OrderItem_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Warranty",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warranty", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiamondRequest",
                columns: table => new
                {
                    DiamondRequestId = table.Column<string>(type: "text", nullable: false),
                    CustomizeRequestId = table.Column<string>(type: "text", nullable: false),
                    DiamondShapeId = table.Column<string>(type: "text", nullable: true),
                    DiamondId = table.Column<string>(type: "text", nullable: true),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    Clarity = table.Column<string>(type: "text", nullable: true),
                    Color = table.Column<string>(type: "text", nullable: true),
                    Cut = table.Column<string>(type: "text", nullable: true),
                    CaratFrom = table.Column<decimal>(type: "numeric", nullable: true),
                    CaratTo = table.Column<decimal>(type: "numeric", nullable: true),
                    IsLabGrown = table.Column<bool>(type: "boolean", nullable: true),
                    Polish = table.Column<string>(type: "text", nullable: true),
                    Symmetry = table.Column<string>(type: "text", nullable: true),
                    Girdle = table.Column<string>(type: "text", nullable: true),
                    Culet = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiamondRequest", x => new { x.DiamondRequestId, x.CustomizeRequestId });
                    table.ForeignKey(
                        name: "FK_DiamondRequest_CustomizeRequest_CustomizeRequestId",
                        column: x => x.CustomizeRequestId,
                        principalTable: "CustomizeRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiamondRequest_Diamond_DiamondId",
                        column: x => x.DiamondId,
                        principalTable: "Diamond",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiamondRequest_Diamond_Shape_DiamondShapeId",
                        column: x => x.DiamondShapeId,
                        principalTable: "Diamond_Shape",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SideDiamondRequest",
                columns: table => new
                {
                    SideDiamondReqId = table.Column<string>(type: "text", nullable: false),
                    CustomizeRequestId = table.Column<string>(type: "text", nullable: false),
                    CaratWeight = table.Column<float>(type: "real", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SideDiamondRequest", x => new { x.SideDiamondReqId, x.CustomizeRequestId });
                    table.ForeignKey(
                        name: "FK_SideDiamondRequest_CustomizeRequest_CustomizeRequestId",
                        column: x => x.CustomizeRequestId,
                        principalTable: "CustomizeRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SideDiamondRequest_SideDiamondReq_SideDiamondReqId",
                        column: x => x.SideDiamondReqId,
                        principalTable: "SideDiamondReq",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_CustomizeRequestId",
                table: "Order",
                column: "CustomizeRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomizeRequest_AccountId",
                table: "CustomizeRequest",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomizeRequest_JewelryModelId",
                table: "CustomizeRequest",
                column: "JewelryModelId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomizeRequest_MetalId",
                table: "CustomizeRequest",
                column: "MetalId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomizeRequest_SizeId",
                table: "CustomizeRequest",
                column: "SizeId");

            migrationBuilder.CreateIndex(
                name: "IX_DiamondRequest_CustomizeRequestId",
                table: "DiamondRequest",
                column: "CustomizeRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_DiamondRequest_DiamondId",
                table: "DiamondRequest",
                column: "DiamondId");

            migrationBuilder.CreateIndex(
                name: "IX_DiamondRequest_DiamondShapeId",
                table: "DiamondRequest",
                column: "DiamondShapeId");

            migrationBuilder.CreateIndex(
                name: "IX_SideDiamondRequest_CustomizeRequestId",
                table: "SideDiamondRequest",
                column: "CustomizeRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_CustomizeRequest_CustomizeRequestId",
                table: "Order",
                column: "CustomizeRequestId",
                principalTable: "CustomizeRequest",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_CustomizeRequest_CustomizeRequestId",
                table: "Order");

            migrationBuilder.DropTable(
                name: "DiamondRequest");

            migrationBuilder.DropTable(
                name: "OrderItemWarranty");

            migrationBuilder.DropTable(
                name: "SideDiamondRequest");

            migrationBuilder.DropTable(
                name: "Warranty");

            migrationBuilder.DropTable(
                name: "CustomizeRequest");

            migrationBuilder.DropIndex(
                name: "IX_Order_CustomizeRequestId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Thumbnail",
                table: "Metal");

            migrationBuilder.DropColumn(
                name: "Gallery",
                table: "JewelryModel");

            migrationBuilder.DropColumn(
                name: "IsLabGrown",
                table: "DiamondCriteria");

            migrationBuilder.DropColumn(
                name: "Gallery",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "Thumbnail",
                table: "Blog");

            migrationBuilder.RenameColumn(
                name: "CustomizeRequestId",
                table: "Order",
                newName: "AccountId1");

            migrationBuilder.AddColumn<string>(
                name: "AccountId1",
                table: "JewelryReview",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DiamondWarranty",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiredDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    WarrantyPath = table.Column<string>(type: "text", nullable: false),
                    WarrantyType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiamondWarranty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiamondWarranty_Diamond_Id",
                        column: x => x.Id,
                        principalTable: "Diamond",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JewelryWarranty",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiredDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    WarrantyPath = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JewelryWarranty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JewelryWarranty_Jewelry_Id",
                        column: x => x.Id,
                        principalTable: "Jewelry",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_AccountId1",
                table: "Order",
                column: "AccountId1");

            migrationBuilder.CreateIndex(
                name: "IX_JewelryReview_AccountId1",
                table: "JewelryReview",
                column: "AccountId1");

            migrationBuilder.CreateIndex(
                name: "IX_JewelryWarranty_Id",
                table: "JewelryWarranty",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JewelryReview_Account_AccountId1",
                table: "JewelryReview",
                column: "AccountId1",
                principalTable: "Account",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Account_AccountId1",
                table: "Order",
                column: "AccountId1",
                principalTable: "Account",
                principalColumn: "Id");
        }
    }
}
