using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOriginToSideDiamond : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLabGrown",
                table: "SideDiamondOpt",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SideDiamond_IsLabGrown",
                table: "Jewelry",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Certificate",
                table: "Diamond",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLabGrown",
                table: "SideDiamondOpt");

            migrationBuilder.DropColumn(
                name: "SideDiamond_IsLabGrown",
                table: "Jewelry");

            migrationBuilder.DropColumn(
                name: "Certificate",
                table: "Diamond");
        }
    }
}
