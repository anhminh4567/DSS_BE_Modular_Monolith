using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Metal",
                columns: new[] { "Id", "Name", "Price" },
                values: new object[,]
                {
                    { "154a9226-9717-447b-9179-de17473857af", "16K Pink Gold", 1391318m },
                    { "22ed1207-1680-45a6-b5c6-ad0dddaad8d6", "18K Pink Gold", 1565233m },
                    { "466c8fdd-2d9e-457e-a1b7-9243c127190b", "18K Yellow Gold", 1565233m },
                    { "6fed2316-cd50-42f6-9d3c-51a1f1452ed7", "14K Yellow Gold", 1217096m },
                    { "784508a2-4516-4147-a209-62f9d77a4e0a", "18K White Gold", 1565233m },
                    { "92e59433-6dd8-4ed6-bb7e-77baed54f682", "16K Yellow Gold", 1391318m },
                    { "c07e4ee6-e592-4da6-b6c9-1c541674adf7", "16K White Gold", 1391318m },
                    { "c4f99f89-6bb3-44fe-972f-08b98592f6b9", "Platinum", 778370m },
                    { "c5f331c6-e99f-4637-afec-aaa5264a184e", "14K White Gold", 1217096m },
                    { "cbedf605-3f46-4cfa-bb55-59afa7b65a66", "14K Pink Gold", 1217096m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Metal_Id",
                table: "Metal",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Metal_Id",
                table: "Metal");

            migrationBuilder.DeleteData(
                table: "Metal",
                keyColumn: "Id",
                keyValue: "154a9226-9717-447b-9179-de17473857af");

            migrationBuilder.DeleteData(
                table: "Metal",
                keyColumn: "Id",
                keyValue: "22ed1207-1680-45a6-b5c6-ad0dddaad8d6");

            migrationBuilder.DeleteData(
                table: "Metal",
                keyColumn: "Id",
                keyValue: "466c8fdd-2d9e-457e-a1b7-9243c127190b");

            migrationBuilder.DeleteData(
                table: "Metal",
                keyColumn: "Id",
                keyValue: "6fed2316-cd50-42f6-9d3c-51a1f1452ed7");

            migrationBuilder.DeleteData(
                table: "Metal",
                keyColumn: "Id",
                keyValue: "784508a2-4516-4147-a209-62f9d77a4e0a");

            migrationBuilder.DeleteData(
                table: "Metal",
                keyColumn: "Id",
                keyValue: "92e59433-6dd8-4ed6-bb7e-77baed54f682");

            migrationBuilder.DeleteData(
                table: "Metal",
                keyColumn: "Id",
                keyValue: "c07e4ee6-e592-4da6-b6c9-1c541674adf7");

            migrationBuilder.DeleteData(
                table: "Metal",
                keyColumn: "Id",
                keyValue: "c4f99f89-6bb3-44fe-972f-08b98592f6b9");

            migrationBuilder.DeleteData(
                table: "Metal",
                keyColumn: "Id",
                keyValue: "c5f331c6-e99f-4637-afec-aaa5264a184e");

            migrationBuilder.DeleteData(
                table: "Metal",
                keyColumn: "Id",
                keyValue: "cbedf605-3f46-4cfa-bb55-59afa7b65a66");
        }
    }
}
