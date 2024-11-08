using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AppProvinces",
                table: "AppProvinces");

            migrationBuilder.DropColumn(
                name: "FromKm",
                table: "DeliveryFee");

            migrationBuilder.DropColumn(
                name: "FromLocation",
                table: "DeliveryFee");

            migrationBuilder.RenameTable(
                name: "AppProvinces",
                newName: "AppProvince");

            migrationBuilder.RenameColumn(
                name: "ToKm",
                table: "DeliveryFee",
                newName: "ToLocationId");

            migrationBuilder.RenameIndex(
                name: "IX_AppProvinces_Name",
                table: "AppProvince",
                newName: "IX_AppProvince_Name");

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "DeliveryFee",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ProvinceId",
                table: "Address",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppProvince",
                table: "AppProvince",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AppCities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCities", x => x.Id);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryFee_ToLocationId",
                table: "DeliveryFee",
                column: "ToLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_AppCities_Name",
                table: "AppCities",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AppCities_Slug",
                table: "AppCities",
                column: "Slug");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryFee_AppCities_ToLocationId",
                table: "DeliveryFee",
                column: "ToLocationId",
                principalTable: "AppCities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryFee_AppCities_ToLocationId",
                table: "DeliveryFee");

            migrationBuilder.DropTable(
                name: "AppCities");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryFee_ToLocationId",
                table: "DeliveryFee");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppProvince",
                table: "AppProvince");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "DeliveryFee");

            migrationBuilder.DropColumn(
                name: "ProvinceId",
                table: "Address");

            migrationBuilder.RenameTable(
                name: "AppProvince",
                newName: "AppProvinces");

            migrationBuilder.RenameColumn(
                name: "ToLocationId",
                table: "DeliveryFee",
                newName: "ToKm");

            migrationBuilder.RenameIndex(
                name: "IX_AppProvince_Name",
                table: "AppProvinces",
                newName: "IX_AppProvinces_Name");

            migrationBuilder.AddColumn<int>(
                name: "FromKm",
                table: "DeliveryFee",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FromLocation",
                table: "DeliveryFee",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppProvinces",
                table: "AppProvinces",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreateDate",
                value: new DateTime(2024, 11, 6, 17, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreateDate",
                value: new DateTime(2024, 11, 6, 17, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreateDate",
                value: new DateTime(2024, 11, 6, 17, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Warranty",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreateDate",
                value: new DateTime(2024, 11, 6, 17, 0, 0, 0, DateTimeKind.Utc));
        }
    }
}
