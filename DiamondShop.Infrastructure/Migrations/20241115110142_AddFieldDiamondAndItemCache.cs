using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldDiamondAndItemCache : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppProvince");

            migrationBuilder.AddColumn<decimal>(
                name: "OriginalPrice",
                table: "OrderItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "CertificateCode",
                table: "Diamond",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CertificateFilePath_ContentType",
                table: "Diamond",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CertificateFilePath_MediaName",
                table: "Diamond",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CertificateFilePath_MediaPath",
                table: "Diamond",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DbCacheModels",
                columns: table => new
                {
                    KeyId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbCacheModels", x => x.KeyId);
                });

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 11, 15, 11, 1, 40, 568, DateTimeKind.Utc).AddTicks(9032));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 11, 15, 11, 1, 40, 568, DateTimeKind.Utc).AddTicks(9250));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreateDate",
                value: new DateTime(2024, 11, 15, 11, 1, 40, 568, DateTimeKind.Utc).AddTicks(9255));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreateDate",
                value: new DateTime(2024, 11, 15, 11, 1, 40, 568, DateTimeKind.Utc).AddTicks(9257));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DbCacheModels");

            migrationBuilder.DropColumn(
                name: "OriginalPrice",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "CertificateCode",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "CertificateFilePath_ContentType",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "CertificateFilePath_MediaName",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "CertificateFilePath_MediaPath",
                table: "Diamond");

            migrationBuilder.CreateTable(
                name: "AppProvince",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ApiId = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppProvince", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AppProvince",
                columns: new[] { "Id", "ApiId", "IsActive", "Name" },
                values: new object[,]
                {
                    { "1", "1", true, "Hà Nội" },
                    { "10", "10", true, "Lào Cai" },
                    { "11", "11", true, "Điện Biên" },
                    { "12", "12", true, "Lai Châu" },
                    { "14", "14", true, "Sơn La" },
                    { "15", "15", true, "Yên Bái" },
                    { "17", "17", true, "Hoà Bình" },
                    { "19", "19", true, "Thái Nguyên" },
                    { "2", "2", true, "Hà Giang" },
                    { "20", "20", true, "Lạng Sơn" },
                    { "22", "22", true, "Quảng Ninh" },
                    { "24", "24", true, "Bắc Giang" },
                    { "25", "25", true, "Phú Thọ" },
                    { "26", "26", true, "Vĩnh Phúc" },
                    { "27", "27", true, "Bắc Ninh" },
                    { "30", "30", true, "Hải Dương" },
                    { "31", "31", true, "Hải Phòng" },
                    { "33", "33", true, "Hưng Yên" },
                    { "34", "34", true, "Thái Bình" },
                    { "35", "35", true, "Hà Nam" },
                    { "36", "36", true, "Nam Định" },
                    { "37", "37", true, "Ninh Bình" },
                    { "38", "38", true, "Thanh Hóa" },
                    { "4", "4", true, "Cao Bằng" },
                    { "40", "40", true, "Nghệ An" },
                    { "42", "42", true, "Hà Tĩnh" },
                    { "44", "44", true, "Quảng Bình" },
                    { "45", "45", true, "Quảng Trị" },
                    { "46", "46", true, "Thừa Thiên Huế" },
                    { "48", "48", true, "Đà Nẵng" },
                    { "49", "49", true, "Quảng Nam" },
                    { "51", "51", true, "Quảng Ngãi" },
                    { "52", "52", true, "Bình Định" },
                    { "54", "54", true, "Phú Yên" },
                    { "56", "56", true, "Khánh Hòa" },
                    { "58", "58", true, "Ninh Thuận" },
                    { "6", "6", true, "Bắc Kạn" },
                    { "60", "60", true, "Bình Thuận" },
                    { "62", "62", true, "Kon Tum" },
                    { "64", "64", true, "Gia Lai" },
                    { "66", "66", true, "Đắk Lắk" },
                    { "67", "67", true, "Đắk Nông" },
                    { "68", "68", true, "Lâm Đồng" },
                    { "70", "70", true, "Bình Phước" },
                    { "72", "72", true, "Tây Ninh" },
                    { "74", "74", true, "Bình Dương" },
                    { "75", "75", true, "Đồng Nai" },
                    { "77", "77", true, "Bà Rịa - Vũng Tàu" },
                    { "79", "79", true, "Hồ Chí Minh" },
                    { "8", "8", true, "Tuyên Quang" },
                    { "80", "80", true, "Long An" },
                    { "82", "82", true, "Tiền Giang" },
                    { "83", "83", true, "Bến Tre" },
                    { "84", "84", true, "Trà Vinh" },
                    { "86", "86", true, "Vĩnh Long" },
                    { "87", "87", true, "Đồng Tháp" },
                    { "89", "89", true, "An Giang" },
                    { "91", "91", true, "Kiên Giang" },
                    { "92", "92", true, "Cần Thơ" },
                    { "93", "93", true, "Hậu Giang" },
                    { "94", "94", true, "Sóc Trăng" },
                    { "95", "95", true, "Bạc Liêu" },
                    { "96", "96", true, "Cà Mau" }
                });

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 11, 15, 7, 35, 14, 355, DateTimeKind.Utc).AddTicks(2608));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 11, 15, 7, 35, 14, 355, DateTimeKind.Utc).AddTicks(2869));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreateDate",
                value: new DateTime(2024, 11, 15, 7, 35, 14, 355, DateTimeKind.Utc).AddTicks(2873));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreateDate",
                value: new DateTime(2024, 11, 15, 7, 35, 14, 355, DateTimeKind.Utc).AddTicks(2874));

            migrationBuilder.CreateIndex(
                name: "IX_AppProvince_Name",
                table: "AppProvince",
                column: "Name");
        }
    }
}
