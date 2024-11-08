using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewId",
                table: "Jewelry");

            migrationBuilder.RenameColumn(
                name: "Images",
                table: "JewelryReview",
                newName: "Medias");

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 11, 8, 11, 11, 14, 145, DateTimeKind.Utc).AddTicks(6826));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 11, 8, 11, 11, 14, 145, DateTimeKind.Utc).AddTicks(7506));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreateDate",
                value: new DateTime(2024, 11, 8, 11, 11, 14, 145, DateTimeKind.Utc).AddTicks(7513));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreateDate",
                value: new DateTime(2024, 11, 8, 11, 11, 14, 145, DateTimeKind.Utc).AddTicks(7515));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Medias",
                table: "JewelryReview",
                newName: "Images");

            migrationBuilder.AddColumn<string>(
                name: "ReviewId",
                table: "Jewelry",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 11, 8, 4, 29, 57, 964, DateTimeKind.Utc).AddTicks(5546));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 11, 8, 4, 29, 57, 964, DateTimeKind.Utc).AddTicks(5923));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreateDate",
                value: new DateTime(2024, 11, 8, 4, 29, 57, 964, DateTimeKind.Utc).AddTicks(5929));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreateDate",
                value: new DateTime(2024, 11, 8, 4, 29, 57, 964, DateTimeKind.Utc).AddTicks(5932));
        }
    }
}
