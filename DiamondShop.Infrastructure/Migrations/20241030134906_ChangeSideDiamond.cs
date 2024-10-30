using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSideDiamond : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SideDiamondOpt_SideDiamondReq_SideDiamondReqId",
                table: "SideDiamondOpt");

            migrationBuilder.DropTable(
                name: "JewelrySideDiamond");

            migrationBuilder.DropTable(
                name: "SideDiamondRequest");

            migrationBuilder.DropTable(
                name: "SideDiamondReq");

            migrationBuilder.DropColumn(
                name: "FinalPrice",
                table: "OrderItem");

            migrationBuilder.RenameColumn(
                name: "SideDiamondReqId",
                table: "SideDiamondOpt",
                newName: "ShapeId");

            migrationBuilder.RenameIndex(
                name: "IX_SideDiamondOpt_SideDiamondReqId",
                table: "SideDiamondOpt",
                newName: "IX_SideDiamondOpt_ShapeId");

            migrationBuilder.AddColumn<int>(
                name: "ClarityMax",
                table: "SideDiamondOpt",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClarityMin",
                table: "SideDiamondOpt",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ColorMax",
                table: "SideDiamondOpt",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ColorMin",
                table: "SideDiamondOpt",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ModelId",
                table: "SideDiamondOpt",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SettingType",
                table: "SideDiamondOpt",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "Carat",
                table: "Jewelry",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClarityMax",
                table: "Jewelry",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClarityMin",
                table: "Jewelry",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorMax",
                table: "Jewelry",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorMin",
                table: "Jewelry",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiamondShapeId",
                table: "Jewelry",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Jewelry",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SD_Price",
                table: "Jewelry",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SettingType",
                table: "Jewelry",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SideDiamondId",
                table: "CustomizeRequest",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "CreateDate", "Type" },
                values: new object[] { new DateTime(2024, 10, 29, 17, 0, 0, 0, DateTimeKind.Utc), "Jewelry" });

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "CreateDate", "Type" },
                values: new object[] { new DateTime(2024, 10, 29, 17, 0, 0, 0, DateTimeKind.Utc), "Diamond" });

            migrationBuilder.CreateIndex(
                name: "IX_SideDiamondOpt_ModelId",
                table: "SideDiamondOpt",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Jewelry_DiamondShapeId",
                table: "Jewelry",
                column: "DiamondShapeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomizeRequest_SideDiamondId",
                table: "CustomizeRequest",
                column: "SideDiamondId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomizeRequest_SideDiamondOpt_SideDiamondId",
                table: "CustomizeRequest",
                column: "SideDiamondId",
                principalTable: "SideDiamondOpt",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Jewelry_Diamond_Shape_DiamondShapeId",
                table: "Jewelry",
                column: "DiamondShapeId",
                principalTable: "Diamond_Shape",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SideDiamondOpt_Diamond_Shape_ShapeId",
                table: "SideDiamondOpt",
                column: "ShapeId",
                principalTable: "Diamond_Shape",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SideDiamondOpt_JewelryModel_ModelId",
                table: "SideDiamondOpt",
                column: "ModelId",
                principalTable: "JewelryModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomizeRequest_SideDiamondOpt_SideDiamondId",
                table: "CustomizeRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_Jewelry_Diamond_Shape_DiamondShapeId",
                table: "Jewelry");

            migrationBuilder.DropForeignKey(
                name: "FK_SideDiamondOpt_Diamond_Shape_ShapeId",
                table: "SideDiamondOpt");

            migrationBuilder.DropForeignKey(
                name: "FK_SideDiamondOpt_JewelryModel_ModelId",
                table: "SideDiamondOpt");

            migrationBuilder.DropIndex(
                name: "IX_SideDiamondOpt_ModelId",
                table: "SideDiamondOpt");

            migrationBuilder.DropIndex(
                name: "IX_Jewelry_DiamondShapeId",
                table: "Jewelry");

            migrationBuilder.DropIndex(
                name: "IX_CustomizeRequest_SideDiamondId",
                table: "CustomizeRequest");

            migrationBuilder.DropColumn(
                name: "ClarityMax",
                table: "SideDiamondOpt");

            migrationBuilder.DropColumn(
                name: "ClarityMin",
                table: "SideDiamondOpt");

            migrationBuilder.DropColumn(
                name: "ColorMax",
                table: "SideDiamondOpt");

            migrationBuilder.DropColumn(
                name: "ColorMin",
                table: "SideDiamondOpt");

            migrationBuilder.DropColumn(
                name: "ModelId",
                table: "SideDiamondOpt");

            migrationBuilder.DropColumn(
                name: "SettingType",
                table: "SideDiamondOpt");

            migrationBuilder.DropColumn(
                name: "Carat",
                table: "Jewelry");

            migrationBuilder.DropColumn(
                name: "ClarityMax",
                table: "Jewelry");

            migrationBuilder.DropColumn(
                name: "ClarityMin",
                table: "Jewelry");

            migrationBuilder.DropColumn(
                name: "ColorMax",
                table: "Jewelry");

            migrationBuilder.DropColumn(
                name: "ColorMin",
                table: "Jewelry");

            migrationBuilder.DropColumn(
                name: "DiamondShapeId",
                table: "Jewelry");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Jewelry");

            migrationBuilder.DropColumn(
                name: "SD_Price",
                table: "Jewelry");

            migrationBuilder.DropColumn(
                name: "SettingType",
                table: "Jewelry");

            migrationBuilder.DropColumn(
                name: "SideDiamondId",
                table: "CustomizeRequest");

            migrationBuilder.RenameColumn(
                name: "ShapeId",
                table: "SideDiamondOpt",
                newName: "SideDiamondReqId");

            migrationBuilder.RenameIndex(
                name: "IX_SideDiamondOpt_ShapeId",
                table: "SideDiamondOpt",
                newName: "IX_SideDiamondOpt_SideDiamondReqId");

            migrationBuilder.AddColumn<decimal>(
                name: "FinalPrice",
                table: "OrderItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "JewelrySideDiamond",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DiamondShapeId = table.Column<string>(type: "text", nullable: true),
                    Carat = table.Column<float>(type: "real", nullable: false),
                    ClarityMax = table.Column<int>(type: "integer", nullable: false),
                    ClarityMin = table.Column<int>(type: "integer", nullable: false),
                    ColorMax = table.Column<int>(type: "integer", nullable: false),
                    ColorMin = table.Column<int>(type: "integer", nullable: false),
                    JewelryId = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    SettingType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JewelrySideDiamond", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JewelrySideDiamond_Diamond_Shape_DiamondShapeId",
                        column: x => x.DiamondShapeId,
                        principalTable: "Diamond_Shape",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JewelrySideDiamond_Jewelry_JewelryId",
                        column: x => x.JewelryId,
                        principalTable: "Jewelry",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SideDiamondReq",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ModelId = table.Column<string>(type: "text", nullable: false),
                    ShapeId = table.Column<string>(type: "text", nullable: false),
                    ClarityMax = table.Column<int>(type: "integer", nullable: false),
                    ClarityMin = table.Column<int>(type: "integer", nullable: false),
                    ColorMax = table.Column<int>(type: "integer", nullable: false),
                    ColorMin = table.Column<int>(type: "integer", nullable: false),
                    SettingType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SideDiamondReq", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SideDiamondReq_Diamond_Shape_ShapeId",
                        column: x => x.ShapeId,
                        principalTable: "Diamond_Shape",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SideDiamondReq_JewelryModel_ModelId",
                        column: x => x.ModelId,
                        principalTable: "JewelryModel",
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

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "CreateDate", "Type" },
                values: new object[] { new DateTime(2024, 10, 27, 17, 0, 0, 0, DateTimeKind.Utc), "0" });

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "CreateDate", "Type" },
                values: new object[] { new DateTime(2024, 10, 27, 17, 0, 0, 0, DateTimeKind.Utc), "0" });

            migrationBuilder.CreateIndex(
                name: "IX_JewelrySideDiamond_DiamondShapeId",
                table: "JewelrySideDiamond",
                column: "DiamondShapeId");

            migrationBuilder.CreateIndex(
                name: "IX_JewelrySideDiamond_JewelryId",
                table: "JewelrySideDiamond",
                column: "JewelryId");

            migrationBuilder.CreateIndex(
                name: "IX_SideDiamondReq_ModelId",
                table: "SideDiamondReq",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_SideDiamondReq_ShapeId",
                table: "SideDiamondReq",
                column: "ShapeId");

            migrationBuilder.CreateIndex(
                name: "IX_SideDiamondRequest_CustomizeRequestId",
                table: "SideDiamondRequest",
                column: "CustomizeRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_SideDiamondOpt_SideDiamondReq_SideDiamondReqId",
                table: "SideDiamondOpt",
                column: "SideDiamondReqId",
                principalTable: "SideDiamondReq",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
