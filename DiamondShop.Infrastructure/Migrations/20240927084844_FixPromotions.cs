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

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Promotion",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Promotion",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "PromoReq",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

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

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "PromoReq",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "UnitValue",
            //    table: "Gift",
            //    type: "numeric",
            //    nullable: false,
            //    oldClrType: typeof(string),
            //    oldType: "text");
            migrationBuilder.Sql(@"
                ALTER TABLE ""Gift""
                ALTER COLUMN ""UnitValue"" TYPE numeric
                USING ""UnitValue""::numeric;
            ");

            migrationBuilder.AlterColumn<string>(
                name: "PromotionId",
                table: "Gift",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ItemId",
                table: "Gift",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "Amount",
                table: "Gift",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<float>(
                name: "CaratFrom",
                table: "Gift",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "CaratTo",
                table: "Gift",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClarityFrom",
                table: "Gift",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClarityTo",
                table: "Gift",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorFrom",
                table: "Gift",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorTo",
                table: "Gift",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CutFrom",
                table: "Gift",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CutTo",
                table: "Gift",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiamondGiftShapes",
                table: "Gift",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiamondOrigin",
                table: "Gift",
                type: "text",
                nullable: true);

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

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Promotion");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Promotion");

            migrationBuilder.DropColumn(
                name: "CaratFrom",
                table: "Gift");

            migrationBuilder.DropColumn(
                name: "CaratTo",
                table: "Gift");

            migrationBuilder.DropColumn(
                name: "ClarityFrom",
                table: "Gift");

            migrationBuilder.DropColumn(
                name: "ClarityTo",
                table: "Gift");

            migrationBuilder.DropColumn(
                name: "ColorFrom",
                table: "Gift");

            migrationBuilder.DropColumn(
                name: "ColorTo",
                table: "Gift");

            migrationBuilder.DropColumn(
                name: "CutFrom",
                table: "Gift");

            migrationBuilder.DropColumn(
                name: "CutTo",
                table: "Gift");

            migrationBuilder.DropColumn(
                name: "DiamondGiftShapes",
                table: "Gift");

            migrationBuilder.DropColumn(
                name: "DiamondOrigin",
                table: "Gift");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "PromoReq",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

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

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "PromoReq",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UnitValue",
                table: "Gift",
                type: "text",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "PromotionId",
                table: "Gift",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ItemId",
                table: "Gift",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Gift",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_PromoReq_JewelryModel_ModelId",
                table: "PromoReq",
                column: "ModelId",
                principalTable: "JewelryModel",
                principalColumn: "Id");
        }
    }
}
