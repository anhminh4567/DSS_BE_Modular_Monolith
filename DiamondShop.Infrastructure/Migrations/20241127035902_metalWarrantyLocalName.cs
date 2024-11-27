using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class metalWarrantyLocalName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LocalizedName",
                table: "Warranty",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LocalizedName",
                table: "Metal",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Metal",
                keyColumn: "Id",
                keyValue: "1",
                column: "LocalizedName",
                value: "Bạch kim");

            migrationBuilder.UpdateData(
                table: "Metal",
                keyColumn: "Id",
                keyValue: "10",
                column: "LocalizedName",
                value: "Vàng hồng 18K");

            migrationBuilder.UpdateData(
                table: "Metal",
                keyColumn: "Id",
                keyValue: "2",
                column: "LocalizedName",
                value: "Vàng 14K");

            migrationBuilder.UpdateData(
                table: "Metal",
                keyColumn: "Id",
                keyValue: "3",
                column: "LocalizedName",
                value: "Vàng trắng 14k");

            migrationBuilder.UpdateData(
                table: "Metal",
                keyColumn: "Id",
                keyValue: "4",
                column: "LocalizedName",
                value: "Vàng hồng 14K");

            migrationBuilder.UpdateData(
                table: "Metal",
                keyColumn: "Id",
                keyValue: "5",
                column: "LocalizedName",
                value: "Vàng 16K");

            migrationBuilder.UpdateData(
                table: "Metal",
                keyColumn: "Id",
                keyValue: "6",
                column: "LocalizedName",
                value: "Vàng trắng 16K");

            migrationBuilder.UpdateData(
                table: "Metal",
                keyColumn: "Id",
                keyValue: "7",
                column: "LocalizedName",
                value: "Vàng hồng 16K");

            migrationBuilder.UpdateData(
                table: "Metal",
                keyColumn: "Id",
                keyValue: "8",
                column: "LocalizedName",
                value: "Vàng 18K");

            migrationBuilder.UpdateData(
                table: "Metal",
                keyColumn: "Id",
                keyValue: "9",
                column: "LocalizedName",
                value: "Vàng trắng 18K");

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "CreateDate", "LocalizedName" },
                values: new object[] { new DateTime(2024, 11, 27, 3, 58, 59, 761, DateTimeKind.Utc).AddTicks(3957), "Bảo hành trang sức 3 tháng" });

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "CreateDate", "LocalizedName" },
                values: new object[] { new DateTime(2024, 11, 27, 3, 58, 59, 761, DateTimeKind.Utc).AddTicks(4225), "Bảo hành kim cương 3 tháng" });

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3",
                columns: new[] { "CreateDate", "LocalizedName" },
                values: new object[] { new DateTime(2024, 11, 27, 3, 58, 59, 761, DateTimeKind.Utc).AddTicks(4231), "Bảo hành trang sức 1 năm" });

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4",
                columns: new[] { "CreateDate", "LocalizedName" },
                values: new object[] { new DateTime(2024, 11, 27, 3, 58, 59, 761, DateTimeKind.Utc).AddTicks(4232), "Bảo hành trang sức 1 năm" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocalizedName",
                table: "Warranty");

            migrationBuilder.DropColumn(
                name: "LocalizedName",
                table: "Metal");

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 11, 23, 9, 29, 21, 740, DateTimeKind.Utc).AddTicks(3586));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 11, 23, 9, 29, 21, 740, DateTimeKind.Utc).AddTicks(3788));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreateDate",
                value: new DateTime(2024, 11, 23, 9, 29, 21, 740, DateTimeKind.Utc).AddTicks(3795));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreateDate",
                value: new DateTime(2024, 11, 23, 9, 29, 21, 740, DateTimeKind.Utc).AddTicks(3797));
        }
    }
}
