using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBlog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gallery",
                table: "Diamond");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "Medias",
                table: "Blog");

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 11, 11, 17, 57, 42, 412, DateTimeKind.Utc).AddTicks(35));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 11, 11, 17, 57, 42, 412, DateTimeKind.Utc).AddTicks(299));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreateDate",
                value: new DateTime(2024, 11, 11, 17, 57, 42, 412, DateTimeKind.Utc).AddTicks(304));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreateDate",
                value: new DateTime(2024, 11, 11, 17, 57, 42, 412, DateTimeKind.Utc).AddTicks(306));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Gallery",
                table: "Diamond",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Blog",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Medias",
                table: "Blog",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 11, 9, 12, 17, 28, 88, DateTimeKind.Utc).AddTicks(2160));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 11, 9, 12, 17, 28, 88, DateTimeKind.Utc).AddTicks(2390));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreateDate",
                value: new DateTime(2024, 11, 9, 12, 17, 28, 88, DateTimeKind.Utc).AddTicks(2395));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreateDate",
                value: new DateTime(2024, 11, 9, 12, 17, 28, 88, DateTimeKind.Utc).AddTicks(2398));
        }
    }
}
