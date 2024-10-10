using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRedundantIndexs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Size_Id",
                table: "Size");

            migrationBuilder.DropIndex(
                name: "IX_JewelrySideDiamond_Id",
                table: "JewelrySideDiamond");

            migrationBuilder.DropIndex(
                name: "IX_JewelryModel_Id",
                table: "JewelryModel");

            migrationBuilder.DropIndex(
                name: "IX_Jewelry_Id",
                table: "Jewelry");

            migrationBuilder.DropIndex(
                name: "IX_Diamond_Shape_Id",
                table: "Diamond_Shape");

            migrationBuilder.DropIndex(
                name: "IX_Diamond_DiamondShapeId",
                table: "Diamond");

            migrationBuilder.DropIndex(
                name: "IX_Diamond_Id",
                table: "Diamond");

            migrationBuilder.DropIndex(
                name: "IX_Account_Id",
                table: "Account");

            migrationBuilder.CreateIndex(
                name: "IX_Diamond_DiamondShapeId",
                table: "Diamond",
                column: "DiamondShapeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Diamond_DiamondShapeId",
                table: "Diamond");

            migrationBuilder.CreateIndex(
                name: "IX_Size_Id",
                table: "Size",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_JewelrySideDiamond_Id",
                table: "JewelrySideDiamond",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_JewelryModel_Id",
                table: "JewelryModel",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Jewelry_Id",
                table: "Jewelry",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Diamond_Shape_Id",
                table: "Diamond_Shape",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Diamond_DiamondShapeId",
                table: "Diamond",
                column: "DiamondShapeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Diamond_Id",
                table: "Diamond",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Account_Id",
                table: "Account",
                column: "Id");
        }
    }
}
