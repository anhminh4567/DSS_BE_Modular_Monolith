using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateSizeSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "044bce84-5489-4b29-a5c7-c8bad1d809e7");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "0ff62177-8a43-4b6a-b030-c61ca2a2a7fa");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "1e4d3bdc-e750-4035-8d0c-96ebcb44b52a");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "2cf26de6-c5ed-48bb-b958-ed4ccfe0185d");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "58f05627-64ff-4f42-ba16-885407606e1c");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "5d5e3611-7a26-4669-b711-22c18e0ac509");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "6f8397d3-c8f0-4567-8af7-673bba2c2350");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "7513a30e-493e-4ccb-ad4c-5638b5ec2aaf");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "860820eb-2a33-4577-a960-06da7a56eb71");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "8e5a6836-9f1f-460c-9f20-4c1b487f33f3");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "9547adea-e1d8-4b94-97f9-4d5fb0949b02");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "99de9fbd-56a8-413c-b6d2-db448f26e15f");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "9b4ab47f-6fd9-40d6-b9f3-1c4988dfa066");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "9bd741d2-70f3-4a83-8ead-057e2ca9c3a0");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "9cbf45f1-e76c-41c4-8489-ada66d5f6a6b");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "b3b433fa-f2c3-489d-9a79-35f35c3c265a");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "c5593cf5-e154-4113-8588-cca21286187c");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "d239cf5a-6feb-4a61-bd65-b07f1881bf86");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "d6aea828-a5e0-47ca-9cc3-6a6ae72ecacc");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "e7a2a6a9-8d98-4946-bfbd-4a53cbc26767");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "e97bd250-97b7-428a-8d37-bf430e5a280e");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "f9602eac-5a21-48bb-a765-a7129d43b19d");

            migrationBuilder.UpdateData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "10",
                column: "Value",
                value: 10f);

            migrationBuilder.UpdateData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "3",
                column: "Value",
                value: 3f);

            migrationBuilder.UpdateData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "4",
                column: "Value",
                value: 4f);

            migrationBuilder.UpdateData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "5",
                column: "Value",
                value: 5f);

            migrationBuilder.UpdateData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "6",
                column: "Value",
                value: 6f);

            migrationBuilder.UpdateData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "7",
                column: "Value",
                value: 7f);

            migrationBuilder.UpdateData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "8",
                column: "Value",
                value: 8f);

            migrationBuilder.UpdateData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "9",
                column: "Value",
                value: 9f);

            migrationBuilder.InsertData(
                table: "Size",
                columns: new[] { "Id", "Unit", "Value" },
                values: new object[,]
                {
                    { "11", "milimeter", 11f },
                    { "12", "milimeter", 12f },
                    { "13", "milimeter", 13f },
                    { "14", "milimeter", 14f },
                    { "15", "milimeter", 15f },
                    { "16", "milimeter", 16f },
                    { "17", "milimeter", 17f },
                    { "18", "milimeter", 18f },
                    { "19", "milimeter", 19f },
                    { "20", "milimeter", 20f },
                    { "21", "milimeter", 21f },
                    { "22", "milimeter", 22f },
                    { "23", "milimeter", 23f },
                    { "24", "milimeter", 24f }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "11");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "12");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "13");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "14");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "15");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "16");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "17");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "18");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "19");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "20");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "21");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "22");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "23");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "24");

            migrationBuilder.UpdateData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "10",
                column: "Value",
                value: 12f);

            migrationBuilder.UpdateData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "3",
                column: "Value",
                value: 5f);

            migrationBuilder.UpdateData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "4",
                column: "Value",
                value: 6f);

            migrationBuilder.UpdateData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "5",
                column: "Value",
                value: 7f);

            migrationBuilder.UpdateData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "6",
                column: "Value",
                value: 8f);

            migrationBuilder.UpdateData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "7",
                column: "Value",
                value: 9f);

            migrationBuilder.UpdateData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "8",
                column: "Value",
                value: 10f);

            migrationBuilder.UpdateData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "9",
                column: "Value",
                value: 11f);

            migrationBuilder.InsertData(
                table: "Size",
                columns: new[] { "Id", "Unit", "Value" },
                values: new object[,]
                {
                    { "044bce84-5489-4b29-a5c7-c8bad1d809e7", "milimeter", 17f },
                    { "0ff62177-8a43-4b6a-b030-c61ca2a2a7fa", "milimeter", 5f },
                    { "1", "milimeter", 3f },
                    { "1e4d3bdc-e750-4035-8d0c-96ebcb44b52a", "milimeter", 8f },
                    { "2", "milimeter", 4f },
                    { "2cf26de6-c5ed-48bb-b958-ed4ccfe0185d", "milimeter", 11f },
                    { "58f05627-64ff-4f42-ba16-885407606e1c", "milimeter", 18f },
                    { "5d5e3611-7a26-4669-b711-22c18e0ac509", "milimeter", 16f },
                    { "6f8397d3-c8f0-4567-8af7-673bba2c2350", "milimeter", 15f },
                    { "7513a30e-493e-4ccb-ad4c-5638b5ec2aaf", "milimeter", 4f },
                    { "860820eb-2a33-4577-a960-06da7a56eb71", "milimeter", 12f },
                    { "8e5a6836-9f1f-460c-9f20-4c1b487f33f3", "milimeter", 14f },
                    { "9547adea-e1d8-4b94-97f9-4d5fb0949b02", "milimeter", 13f },
                    { "99de9fbd-56a8-413c-b6d2-db448f26e15f", "milimeter", 22f },
                    { "9b4ab47f-6fd9-40d6-b9f3-1c4988dfa066", "milimeter", 7f },
                    { "9bd741d2-70f3-4a83-8ead-057e2ca9c3a0", "milimeter", 23f },
                    { "9cbf45f1-e76c-41c4-8489-ada66d5f6a6b", "milimeter", 3f },
                    { "b3b433fa-f2c3-489d-9a79-35f35c3c265a", "milimeter", 20f },
                    { "c5593cf5-e154-4113-8588-cca21286187c", "milimeter", 10f },
                    { "d239cf5a-6feb-4a61-bd65-b07f1881bf86", "milimeter", 21f },
                    { "d6aea828-a5e0-47ca-9cc3-6a6ae72ecacc", "milimeter", 9f },
                    { "e7a2a6a9-8d98-4946-bfbd-4a53cbc26767", "milimeter", 19f },
                    { "e97bd250-97b7-428a-8d37-bf430e5a280e", "milimeter", 6f },
                    { "f9602eac-5a21-48bb-a765-a7129d43b19d", "milimeter", 24f }
                });
        }
    }
}
