using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RequestJewelry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModelCode",
                table: "JewelryModel",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "JewelryId",
                table: "Diamond",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JewelryId",
                table: "CustomizeRequest",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_CustomizeRequest_JewelryId",
                table: "CustomizeRequest",
                column: "JewelryId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomizeRequest_Jewelry_JewelryId",
                table: "CustomizeRequest",
                column: "JewelryId",
                principalTable: "Jewelry",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomizeRequest_Jewelry_JewelryId",
                table: "CustomizeRequest");

            migrationBuilder.DropIndex(
                name: "IX_CustomizeRequest_JewelryId",
                table: "CustomizeRequest");

            migrationBuilder.DropColumn(
                name: "ModelCode",
                table: "JewelryModel");

            migrationBuilder.DropColumn(
                name: "JewelryId",
                table: "CustomizeRequest");

            migrationBuilder.AlterColumn<string>(
                name: "JewelryId",
                table: "Diamond",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
