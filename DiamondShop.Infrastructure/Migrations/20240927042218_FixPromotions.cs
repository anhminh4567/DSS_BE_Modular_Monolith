using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPromotions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PromoReq_JewelryModel_ModelId",
                table: "PromoReq");

            migrationBuilder.AlterColumn<string>(
                name: "PromotionId",
                table: "PromoReq",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "DiscountId",
                table: "PromoReq",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "PromotionId",
                table: "Gift",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_PromoReq_JewelryModel_ModelId",
                table: "PromoReq",
                column: "ModelId",
                principalTable: "JewelryModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PromoReq_JewelryModel_ModelId",
                table: "PromoReq");

            migrationBuilder.AlterColumn<string>(
                name: "PromotionId",
                table: "PromoReq",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DiscountId",
                table: "PromoReq",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PromotionId",
                table: "Gift",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PromoReq_JewelryModel_ModelId",
                table: "PromoReq",
                column: "ModelId",
                principalTable: "JewelryModel",
                principalColumn: "Id");
        }
    }
}
