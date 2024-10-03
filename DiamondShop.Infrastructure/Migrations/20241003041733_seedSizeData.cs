using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class seedSizeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Thumbnail",
                table: "Metal");

            migrationBuilder.AddColumn<string>(
                name: "Thumbnail_ContentType",
                table: "Metal",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Thumbnail_MediaName",
                table: "Metal",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Thumbnail_MediaPath",
                table: "Metal",
                type: "text",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Metal",
                columns: new[] { "Id", "Name", "Price" },
                values: new object[,]
                {
                    { "0296db20-d597-483f-a835-3b8f5aeac260", "14K White Gold", 1217096m },
                    { "158c188e-a635-441f-a6b5-ba92f5b4a8b2", "14K Pink Gold", 1217096m },
                    { "5b30aec8-7d40-44f1-831d-9bfe1f956e91", "18K White Gold", 1565233m },
                    { "96bd6ee3-5b53-41fc-aa4e-215644833b17", "14K Yellow Gold", 1217096m },
                    { "9b1bbae2-d3de-452c-a699-75f777f28344", "Platinum", 778370m },
                    { "a032378e-1e03-4931-8a9d-b05448c96643", "16K White Gold", 1391318m },
                    { "cf21d220-b836-4320-8a87-d0d15d0c8a78", "18K Yellow Gold", 1565233m },
                    { "d8013842-8f10-41f3-9cac-dfebfe197f87", "16K Yellow Gold", 1391318m },
                    { "de9cfca6-260f-4025-b3db-b6e2e608df01", "16K Pink Gold", 1391318m },
                    { "fca611bf-f706-4038-882a-651f5972ba6d", "18K Pink Gold", 1565233m }
                });

            migrationBuilder.InsertData(
                table: "Size",
                columns: new[] { "Id", "Unit", "Value" },
                values: new object[,]
                {
                    { "158ffe39-c486-4aef-b2b0-2d5f748d7399", "milimeter", 5f },
                    { "1705b58b-7eb4-4625-97cc-d33540816461", "milimeter", 11f },
                    { "1dbc2a5c-10ba-4f66-ac80-ef9ffd9ffc6e", "milimeter", 24f },
                    { "26910ada-6d78-4749-a189-b9df8781823f", "milimeter", 10f },
                    { "2bedddec-7c52-4c27-9885-b0440c8ce6bc", "milimeter", 4f },
                    { "34c34cab-8aa0-42e5-8ed2-e45e43ccb61e", "milimeter", 12f },
                    { "3b8a7a1e-d872-43fd-a6c9-6dca4a502dab", "milimeter", 20f },
                    { "3ee87ad8-ecce-4615-bc46-404fda9a2716", "milimeter", 8f },
                    { "4326f493-9b50-453b-aef8-d7e44fae1004", "milimeter", 22f },
                    { "4d794ca5-ab25-436d-b915-3aa46feb7bd8", "milimeter", 9f },
                    { "58111f37-37bb-4cce-ad1d-bdb05cc79563", "milimeter", 7f },
                    { "67c6bb97-dcb4-4238-8adb-45caa873116b", "milimeter", 21f },
                    { "6e929733-ac78-44ea-bc5d-a78ea353796e", "milimeter", 6f },
                    { "6fb280a4-aa0b-498b-b78a-7f0f93d64b45", "milimeter", 16f },
                    { "7fd5393f-07a4-41ee-b01a-3c094a26c26c", "milimeter", 23f },
                    { "80b8ada4-fbd8-46a7-9015-933da5a2c65a", "milimeter", 14f },
                    { "874f136d-88d1-4d76-b1b7-e927e8ba60ec", "milimeter", 11f },
                    { "92779a8d-2bc6-4040-a9e5-54ea06dba4f1", "milimeter", 6f },
                    { "94068c81-13b1-4729-94db-fb21c1772359", "milimeter", 3f },
                    { "9da4b1f7-40df-4d63-bbb8-2dfe51a000d1", "milimeter", 3f },
                    { "a3ccab2d-10b8-49f3-9fa5-fb048c8d0568", "milimeter", 10f },
                    { "af6e5c22-b7e7-4b47-ba08-9cf20cb90be3", "milimeter", 7f },
                    { "b1b64c3d-d176-41bc-9fba-31cc9daf6b03", "milimeter", 8f },
                    { "b8b13601-d7e9-47db-b924-6b9679dde510", "milimeter", 4f },
                    { "c87f7e47-5f7b-4248-aed2-3551bd46f205", "milimeter", 17f },
                    { "ca5c01a9-a9ac-41ef-bb9f-6de8d5a58de8", "milimeter", 12f },
                    { "ce0587b5-0ec3-4ec4-8586-44757311d275", "milimeter", 18f },
                    { "d4698130-2b66-4ac6-ae0f-a64b11ea4cf3", "milimeter", 19f },
                    { "d5ec0c65-a84f-4f30-94af-db67f9a9ea85", "milimeter", 15f },
                    { "dcaa48e2-522a-447f-9d5b-66fa8bca3632", "milimeter", 5f },
                    { "f36f9582-64e3-4008-95c1-6e2ca14d56c3", "milimeter", 9f },
                    { "fae5f221-86c7-4207-9bf8-7139cbc793b9", "milimeter", 13f }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Size_Id",
                table: "Size",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Metal_Id",
                table: "Metal",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Size_Id",
                table: "Size");

            migrationBuilder.DropIndex(
                name: "IX_Metal_Id",
                table: "Metal");

            migrationBuilder.DeleteData(
                table: "Metal",
                keyColumn: "Id",
                keyValue: "0296db20-d597-483f-a835-3b8f5aeac260");

            migrationBuilder.DeleteData(
                table: "Metal",
                keyColumn: "Id",
                keyValue: "158c188e-a635-441f-a6b5-ba92f5b4a8b2");

            migrationBuilder.DeleteData(
                table: "Metal",
                keyColumn: "Id",
                keyValue: "5b30aec8-7d40-44f1-831d-9bfe1f956e91");

            migrationBuilder.DeleteData(
                table: "Metal",
                keyColumn: "Id",
                keyValue: "96bd6ee3-5b53-41fc-aa4e-215644833b17");

            migrationBuilder.DeleteData(
                table: "Metal",
                keyColumn: "Id",
                keyValue: "9b1bbae2-d3de-452c-a699-75f777f28344");

            migrationBuilder.DeleteData(
                table: "Metal",
                keyColumn: "Id",
                keyValue: "a032378e-1e03-4931-8a9d-b05448c96643");

            migrationBuilder.DeleteData(
                table: "Metal",
                keyColumn: "Id",
                keyValue: "cf21d220-b836-4320-8a87-d0d15d0c8a78");

            migrationBuilder.DeleteData(
                table: "Metal",
                keyColumn: "Id",
                keyValue: "d8013842-8f10-41f3-9cac-dfebfe197f87");

            migrationBuilder.DeleteData(
                table: "Metal",
                keyColumn: "Id",
                keyValue: "de9cfca6-260f-4025-b3db-b6e2e608df01");

            migrationBuilder.DeleteData(
                table: "Metal",
                keyColumn: "Id",
                keyValue: "fca611bf-f706-4038-882a-651f5972ba6d");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "158ffe39-c486-4aef-b2b0-2d5f748d7399");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "1705b58b-7eb4-4625-97cc-d33540816461");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "1dbc2a5c-10ba-4f66-ac80-ef9ffd9ffc6e");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "26910ada-6d78-4749-a189-b9df8781823f");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "2bedddec-7c52-4c27-9885-b0440c8ce6bc");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "34c34cab-8aa0-42e5-8ed2-e45e43ccb61e");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "3b8a7a1e-d872-43fd-a6c9-6dca4a502dab");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "3ee87ad8-ecce-4615-bc46-404fda9a2716");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "4326f493-9b50-453b-aef8-d7e44fae1004");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "4d794ca5-ab25-436d-b915-3aa46feb7bd8");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "58111f37-37bb-4cce-ad1d-bdb05cc79563");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "67c6bb97-dcb4-4238-8adb-45caa873116b");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "6e929733-ac78-44ea-bc5d-a78ea353796e");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "6fb280a4-aa0b-498b-b78a-7f0f93d64b45");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "7fd5393f-07a4-41ee-b01a-3c094a26c26c");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "80b8ada4-fbd8-46a7-9015-933da5a2c65a");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "874f136d-88d1-4d76-b1b7-e927e8ba60ec");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "92779a8d-2bc6-4040-a9e5-54ea06dba4f1");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "94068c81-13b1-4729-94db-fb21c1772359");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "9da4b1f7-40df-4d63-bbb8-2dfe51a000d1");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "a3ccab2d-10b8-49f3-9fa5-fb048c8d0568");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "af6e5c22-b7e7-4b47-ba08-9cf20cb90be3");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "b1b64c3d-d176-41bc-9fba-31cc9daf6b03");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "b8b13601-d7e9-47db-b924-6b9679dde510");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "c87f7e47-5f7b-4248-aed2-3551bd46f205");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "ca5c01a9-a9ac-41ef-bb9f-6de8d5a58de8");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "ce0587b5-0ec3-4ec4-8586-44757311d275");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "d4698130-2b66-4ac6-ae0f-a64b11ea4cf3");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "d5ec0c65-a84f-4f30-94af-db67f9a9ea85");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "dcaa48e2-522a-447f-9d5b-66fa8bca3632");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "f36f9582-64e3-4008-95c1-6e2ca14d56c3");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "fae5f221-86c7-4207-9bf8-7139cbc793b9");

            migrationBuilder.DropColumn(
                name: "Thumbnail_ContentType",
                table: "Metal");

            migrationBuilder.DropColumn(
                name: "Thumbnail_MediaName",
                table: "Metal");

            migrationBuilder.DropColumn(
                name: "Thumbnail_MediaPath",
                table: "Metal");

            migrationBuilder.AddColumn<string>(
                name: "Thumbnail",
                table: "Metal",
                type: "jsonb",
                nullable: true);
        }
    }
}
