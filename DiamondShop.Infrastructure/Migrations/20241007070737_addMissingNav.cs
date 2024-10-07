using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addMissingNav : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MainDiamondShape_MainDiamondReq_MainDiamondReqId1",
                table: "MainDiamondShape");

            migrationBuilder.DropForeignKey(
                name: "FK_PromoReqShape_PromoReq_PromoReqId1",
                table: "PromoReqShape");

            migrationBuilder.DropForeignKey(
                name: "FK_SideDiamondOpt_SideDiamondReq_SideDiamondReqId1",
                table: "SideDiamondOpt");

            migrationBuilder.DropForeignKey(
                name: "FK_SideDiamondReq_JewelryModel_ModelId1",
                table: "SideDiamondReq");

            migrationBuilder.DropForeignKey(
                name: "FK_SizeMetal_JewelryModel_JewelryModelId",
                table: "SizeMetal");

            migrationBuilder.DropIndex(
                name: "IX_SizeMetal_JewelryModelId",
                table: "SizeMetal");

            migrationBuilder.DropIndex(
                name: "IX_SideDiamondReq_ModelId1",
                table: "SideDiamondReq");

            migrationBuilder.DropIndex(
                name: "IX_SideDiamondOpt_SideDiamondReqId1",
                table: "SideDiamondOpt");

            migrationBuilder.DropIndex(
                name: "IX_PromoReqShape_PromoReqId1",
                table: "PromoReqShape");

            migrationBuilder.DropIndex(
                name: "IX_MainDiamondShape_MainDiamondReqId",
                table: "MainDiamondShape");

            migrationBuilder.DropIndex(
                name: "IX_MainDiamondShape_MainDiamondReqId1",
                table: "MainDiamondShape");

            migrationBuilder.DropIndex(
                name: "IX_JewelryModel_CategoryId",
                table: "JewelryModel");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "17f683b8-f15d-41fb-9eb1-f33b06412e77");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "18f25077-96d2-46f0-bf46-314dbe2ad407");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "190d6400-af5a-47a6-9ded-3f6258b800cf");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "26efce5f-7952-4480-a05a-ce0a1334d715");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "36b167cc-d42f-4ab6-95cd-06453a15d773");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "3a4e3119-bcf2-45b3-a1bb-6d51273646ca");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "3d9622ea-4448-4172-96a5-9c55eb6bdd1a");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "4a0cd35c-4f4e-47cb-af78-8cbca797d511");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "5894ca7c-e56c-461a-a155-9e03a423d8c3");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "60d44e05-0689-4161-8384-3bc7cf06bf05");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "66350f7b-6600-4c64-83dc-c4f82da068c2");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "75136cfd-a509-4d45-a0c9-7120caa75743");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "827b229a-7bef-443e-be13-4502208c2caa");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "8fa384d8-71f9-4861-b45e-87454c808bf6");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "a1c94f37-324b-4fc4-b9c4-5f37af8edc39");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "a5305a21-f3c0-4124-8dac-d2b6da162ea4");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "b82d80f8-2a15-49f1-aaa7-675046ffacc8");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "cd9f1304-b7f4-4080-93f5-19e5729ced51");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "d5f55296-ebbc-434c-8a6d-33214741b3dd");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "e8504aaa-7048-4ac9-8a70-ca66ba865926");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "eee909fe-bb68-4090-a621-9cd148e5198e");

            migrationBuilder.DeleteData(
                table: "Size",
                keyColumn: "Id",
                keyValue: "ef82fe7c-3921-48b6-b7aa-d9f2c01ade2e");

            migrationBuilder.DropColumn(
                name: "JewelryModelId",
                table: "SizeMetal");

            migrationBuilder.DropColumn(
                name: "ModelId1",
                table: "SideDiamondReq");

            migrationBuilder.DropColumn(
                name: "SideDiamondReqId1",
                table: "SideDiamondOpt");

            migrationBuilder.DropColumn(
                name: "PromoReqId1",
                table: "PromoReqShape");

            migrationBuilder.DropColumn(
                name: "MainDiamondReqId1",
                table: "MainDiamondShape");

            migrationBuilder.InsertData(
                table: "Size",
                columns: new[] { "Id", "Unit", "Value" },
                values: new object[,]
                {
                    { "044bce84-5489-4b29-a5c7-c8bad1d809e7", "milimeter", 17f },
                    { "0ff62177-8a43-4b6a-b030-c61ca2a2a7fa", "milimeter", 5f },
                    { "1e4d3bdc-e750-4035-8d0c-96ebcb44b52a", "milimeter", 8f },
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

            migrationBuilder.CreateIndex(
                name: "IX_JewelryModel_CategoryId",
                table: "JewelryModel",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JewelryModel_CategoryId",
                table: "JewelryModel");

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
                keyValue: "1e4d3bdc-e750-4035-8d0c-96ebcb44b52a");

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

            migrationBuilder.AddColumn<string>(
                name: "JewelryModelId",
                table: "SizeMetal",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModelId1",
                table: "SideDiamondReq",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SideDiamondReqId1",
                table: "SideDiamondOpt",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PromoReqId1",
                table: "PromoReqShape",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MainDiamondReqId1",
                table: "MainDiamondShape",
                type: "text",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Size",
                columns: new[] { "Id", "Unit", "Value" },
                values: new object[,]
                {
                    { "17f683b8-f15d-41fb-9eb1-f33b06412e77", "milimeter", 18f },
                    { "18f25077-96d2-46f0-bf46-314dbe2ad407", "milimeter", 14f },
                    { "190d6400-af5a-47a6-9ded-3f6258b800cf", "milimeter", 10f },
                    { "26efce5f-7952-4480-a05a-ce0a1334d715", "milimeter", 23f },
                    { "36b167cc-d42f-4ab6-95cd-06453a15d773", "milimeter", 17f },
                    { "3a4e3119-bcf2-45b3-a1bb-6d51273646ca", "milimeter", 11f },
                    { "3d9622ea-4448-4172-96a5-9c55eb6bdd1a", "milimeter", 15f },
                    { "4a0cd35c-4f4e-47cb-af78-8cbca797d511", "milimeter", 3f },
                    { "5894ca7c-e56c-461a-a155-9e03a423d8c3", "milimeter", 7f },
                    { "60d44e05-0689-4161-8384-3bc7cf06bf05", "milimeter", 12f },
                    { "66350f7b-6600-4c64-83dc-c4f82da068c2", "milimeter", 13f },
                    { "75136cfd-a509-4d45-a0c9-7120caa75743", "milimeter", 6f },
                    { "827b229a-7bef-443e-be13-4502208c2caa", "milimeter", 16f },
                    { "8fa384d8-71f9-4861-b45e-87454c808bf6", "milimeter", 24f },
                    { "a1c94f37-324b-4fc4-b9c4-5f37af8edc39", "milimeter", 9f },
                    { "a5305a21-f3c0-4124-8dac-d2b6da162ea4", "milimeter", 19f },
                    { "b82d80f8-2a15-49f1-aaa7-675046ffacc8", "milimeter", 8f },
                    { "cd9f1304-b7f4-4080-93f5-19e5729ced51", "milimeter", 20f },
                    { "d5f55296-ebbc-434c-8a6d-33214741b3dd", "milimeter", 22f },
                    { "e8504aaa-7048-4ac9-8a70-ca66ba865926", "milimeter", 21f },
                    { "eee909fe-bb68-4090-a621-9cd148e5198e", "milimeter", 4f },
                    { "ef82fe7c-3921-48b6-b7aa-d9f2c01ade2e", "milimeter", 5f }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SizeMetal_JewelryModelId",
                table: "SizeMetal",
                column: "JewelryModelId");

            migrationBuilder.CreateIndex(
                name: "IX_SideDiamondReq_ModelId1",
                table: "SideDiamondReq",
                column: "ModelId1");

            migrationBuilder.CreateIndex(
                name: "IX_SideDiamondOpt_SideDiamondReqId1",
                table: "SideDiamondOpt",
                column: "SideDiamondReqId1");

            migrationBuilder.CreateIndex(
                name: "IX_PromoReqShape_PromoReqId1",
                table: "PromoReqShape",
                column: "PromoReqId1");

            migrationBuilder.CreateIndex(
                name: "IX_MainDiamondShape_MainDiamondReqId",
                table: "MainDiamondShape",
                column: "MainDiamondReqId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MainDiamondShape_MainDiamondReqId1",
                table: "MainDiamondShape",
                column: "MainDiamondReqId1");

            migrationBuilder.CreateIndex(
                name: "IX_JewelryModel_CategoryId",
                table: "JewelryModel",
                column: "CategoryId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MainDiamondShape_MainDiamondReq_MainDiamondReqId1",
                table: "MainDiamondShape",
                column: "MainDiamondReqId1",
                principalTable: "MainDiamondReq",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PromoReqShape_PromoReq_PromoReqId1",
                table: "PromoReqShape",
                column: "PromoReqId1",
                principalTable: "PromoReq",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SideDiamondOpt_SideDiamondReq_SideDiamondReqId1",
                table: "SideDiamondOpt",
                column: "SideDiamondReqId1",
                principalTable: "SideDiamondReq",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SideDiamondReq_JewelryModel_ModelId1",
                table: "SideDiamondReq",
                column: "ModelId1",
                principalTable: "JewelryModel",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SizeMetal_JewelryModel_JewelryModelId",
                table: "SizeMetal",
                column: "JewelryModelId",
                principalTable: "JewelryModel",
                principalColumn: "Id");
        }
    }
}
