using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateJewelryAndDiamond : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Jewelry_MetalId",
                table: "Jewelry");

            migrationBuilder.DropIndex(
                name: "IX_Jewelry_ModelId",
                table: "Jewelry");

            migrationBuilder.DropIndex(
                name: "IX_Jewelry_SizeId",
                table: "Jewelry");

            migrationBuilder.DeleteData(
                table: "PaymentMethod",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.RenameColumn(
                name: "HasGIACert",
                table: "Diamond",
                newName: "IsSold");

            migrationBuilder.InsertData(
                table: "PaymentMethod",
                columns: new[] { "Id", "MethodName", "MethodThumbnailPath", "Status" },
                values: new object[] { "2", "ZALOPAY", null, true });

            migrationBuilder.CreateIndex(
                name: "IX_Jewelry_MetalId",
                table: "Jewelry",
                column: "MetalId");

            migrationBuilder.CreateIndex(
                name: "IX_Jewelry_ModelId",
                table: "Jewelry",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Jewelry_SizeId",
                table: "Jewelry",
                column: "SizeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Jewelry_MetalId",
                table: "Jewelry");

            migrationBuilder.DropIndex(
                name: "IX_Jewelry_ModelId",
                table: "Jewelry");

            migrationBuilder.DropIndex(
                name: "IX_Jewelry_SizeId",
                table: "Jewelry");

            migrationBuilder.DeleteData(
                table: "PaymentMethod",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.RenameColumn(
                name: "IsSold",
                table: "Diamond",
                newName: "HasGIACert");

            migrationBuilder.InsertData(
                table: "PaymentMethod",
                columns: new[] { "Id", "MethodName", "MethodThumbnailPath", "Status" },
                values: new object[] { "2", "ZALOPAY", null, true });

            migrationBuilder.CreateIndex(
                name: "IX_Jewelry_MetalId",
                table: "Jewelry",
                column: "MetalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jewelry_ModelId",
                table: "Jewelry",
                column: "ModelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jewelry_SizeId",
                table: "Jewelry",
                column: "SizeId",
                unique: true);
        }
    }
}
