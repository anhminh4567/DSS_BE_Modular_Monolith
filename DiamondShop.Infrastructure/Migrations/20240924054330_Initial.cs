using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DiamondShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Account_Role",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    RoleName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RoleType = table.Column<int>(type: "integer", nullable: false),
                    RoleDescription = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Diamond_Shape",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Shape = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diamond_Shape", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiamondCriteria",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Cut = table.Column<string>(type: "text", nullable: false),
                    Clarity = table.Column<string>(type: "text", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: false),
                    CaratFrom = table.Column<float>(type: "real", nullable: false),
                    CaratTo = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiamondCriteria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Discount",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DiscountCode = table.Column<string>(type: "text", nullable: false),
                    DiscountPercent = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JewelryModelCategory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ThumbnailPath = table.Column<string>(type: "text", nullable: false),
                    IsGeneral = table.Column<bool>(type: "boolean", nullable: false),
                    ParentCategoryId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JewelryModelCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JewelryModelCategory_JewelryModelCategory_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "JewelryModelCategory",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Metal",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metal", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "News",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    PriorityLevel = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_News", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "outbox_message",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    ProcessTime = table.Column<int>(type: "integer", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Exception = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_outbox_message", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethod",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    MethodName = table.Column<string>(type: "text", nullable: false),
                    MethodThumbnailPath = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethod", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Promotion",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    IsExcludeQualifierProduct = table.Column<bool>(type: "boolean", nullable: false),
                    RedemptionMode = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Size",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Unit = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Size", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Dob = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiamondPrice",
                columns: table => new
                {
                    ShapeId = table.Column<string>(type: "text", nullable: false),
                    CriteriaId = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiamondPrice", x => new { x.ShapeId, x.CriteriaId });
                    table.ForeignKey(
                        name: "FK_DiamondPrice_DiamondCriteria_CriteriaId",
                        column: x => x.CriteriaId,
                        principalTable: "DiamondCriteria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiamondPrice_Diamond_Shape_ShapeId",
                        column: x => x.ShapeId,
                        principalTable: "Diamond_Shape",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JewelryModel",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CategoryId = table.Column<string>(type: "text", nullable: false),
                    Width = table.Column<float>(type: "real", nullable: true),
                    Length = table.Column<float>(type: "real", nullable: true),
                    IsEngravable = table.Column<bool>(type: "boolean", nullable: false),
                    IsRhodiumFinish = table.Column<bool>(type: "boolean", nullable: false),
                    BackType = table.Column<string>(type: "text", nullable: true),
                    ClaspType = table.Column<string>(type: "text", nullable: true),
                    ChainType = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JewelryModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JewelryModel_JewelryModelCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "JewelryModelCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transaction",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    PayMethodId = table.Column<string>(type: "text", nullable: false),
                    TransactionType = table.Column<string>(type: "text", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: false),
                    PayDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TransactionCode = table.Column<string>(type: "text", nullable: false),
                    TransactionAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    RefundedTransacId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transaction_PaymentMethod_PayMethodId",
                        column: x => x.PayMethodId,
                        principalTable: "PaymentMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transaction_Transaction_RefundedTransacId",
                        column: x => x.RefundedTransacId,
                        principalTable: "Transaction",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Gift",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    PromotionId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TargetType = table.Column<string>(type: "text", nullable: false),
                    ItemId = table.Column<string>(type: "text", nullable: false),
                    UnitType = table.Column<string>(type: "text", nullable: false),
                    UnitValue = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gift", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gift_Promotion_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "Promotion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    IdentityId = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Account_User_IdentityId",
                        column: x => x.IdentityId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserToken",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ExpiredDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserToken", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserToken_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Jewelry",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ModelId = table.Column<string>(type: "text", nullable: false),
                    SizeId = table.Column<string>(type: "text", nullable: false),
                    MetalId = table.Column<string>(type: "text", nullable: false),
                    Weight = table.Column<float>(type: "real", nullable: false),
                    SerialCode = table.Column<string>(type: "text", nullable: false),
                    IsAwaiting = table.Column<bool>(type: "boolean", nullable: false),
                    IsSold = table.Column<bool>(type: "boolean", nullable: false),
                    ReviewId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jewelry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jewelry_JewelryModel_ModelId",
                        column: x => x.ModelId,
                        principalTable: "JewelryModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Jewelry_Metal_MetalId",
                        column: x => x.MetalId,
                        principalTable: "Metal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Jewelry_Size_SizeId",
                        column: x => x.SizeId,
                        principalTable: "Size",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MainDiamondReq",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ModelId = table.Column<string>(type: "text", nullable: false),
                    SettingType = table.Column<string>(type: "text", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainDiamondReq", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MainDiamondReq_JewelryModel_ModelId",
                        column: x => x.ModelId,
                        principalTable: "JewelryModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromoReq",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    PromotionId = table.Column<string>(type: "text", nullable: false),
                    DiscountId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TargetType = table.Column<string>(type: "text", nullable: false),
                    Operator = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    ModelId = table.Column<string>(type: "text", nullable: true),
                    DiamondOrigin = table.Column<string>(type: "text", nullable: true),
                    CaratFrom = table.Column<float>(type: "real", nullable: true),
                    CaratTo = table.Column<float>(type: "real", nullable: true),
                    ClarityFrom = table.Column<string>(type: "text", nullable: true),
                    ClarityTo = table.Column<string>(type: "text", nullable: true),
                    CutFrom = table.Column<string>(type: "text", nullable: true),
                    CutTo = table.Column<string>(type: "text", nullable: true),
                    ColorFrom = table.Column<string>(type: "text", nullable: true),
                    ColorTo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoReq", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromoReq_Discount_DiscountId",
                        column: x => x.DiscountId,
                        principalTable: "Discount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromoReq_JewelryModel_ModelId",
                        column: x => x.ModelId,
                        principalTable: "JewelryModel",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PromoReq_Promotion_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "Promotion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SideDiamondReq",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ModelId = table.Column<string>(type: "text", nullable: false),
                    ModelId1 = table.Column<string>(type: "text", nullable: true),
                    ShapeId = table.Column<string>(type: "text", nullable: false),
                    ColorMin = table.Column<string>(type: "text", nullable: false),
                    ColorMax = table.Column<string>(type: "text", nullable: false),
                    ClarityMin = table.Column<string>(type: "text", nullable: false),
                    ClarityMax = table.Column<string>(type: "text", nullable: false),
                    SettingType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SideDiamondReq", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SideDiamondReq_Diamond_Shape_ShapeId",
                        column: x => x.ShapeId,
                        principalTable: "Diamond_Shape",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SideDiamondReq_JewelryModel_ModelId",
                        column: x => x.ModelId,
                        principalTable: "JewelryModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SideDiamondReq_JewelryModel_ModelId1",
                        column: x => x.ModelId1,
                        principalTable: "JewelryModel",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SizeMetal",
                columns: table => new
                {
                    SizeId = table.Column<string>(type: "text", nullable: false),
                    MetalId = table.Column<string>(type: "text", nullable: false),
                    ModelId = table.Column<string>(type: "text", nullable: false),
                    Weight = table.Column<float>(type: "real", nullable: false),
                    JewelryModelId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SizeMetal", x => new { x.SizeId, x.MetalId, x.ModelId });
                    table.ForeignKey(
                        name: "FK_SizeMetal_JewelryModel_JewelryModelId",
                        column: x => x.JewelryModelId,
                        principalTable: "JewelryModel",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SizeMetal_JewelryModel_ModelId",
                        column: x => x.ModelId,
                        principalTable: "JewelryModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SizeMetal_Metal_MetalId",
                        column: x => x.MetalId,
                        principalTable: "Metal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SizeMetal_Size_SizeId",
                        column: x => x.SizeId,
                        principalTable: "Size",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountAccountRole",
                columns: table => new
                {
                    AccountsId = table.Column<string>(type: "text", nullable: false),
                    RolesId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountAccountRole", x => new { x.AccountsId, x.RolesId });
                    table.ForeignKey(
                        name: "FK_AccountAccountRole_Account_AccountsId",
                        column: x => x.AccountsId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountAccountRole_Account_Role_RolesId",
                        column: x => x.RolesId,
                        principalTable: "Account_Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    AccountId = table.Column<string>(type: "text", nullable: false),
                    Province = table.Column<string>(type: "text", nullable: false),
                    District = table.Column<string>(type: "text", nullable: false),
                    Ward = table.Column<string>(type: "text", nullable: false),
                    Street = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Address_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Blog",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Tags = table.Column<string>(type: "text", nullable: true),
                    AccountId = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Medias = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Blog_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    AccountId = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpectedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ShippedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CancelledDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CancelledReason = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    PaymentStatus = table.Column<string>(type: "text", nullable: false),
                    ShippingFee = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalRefund = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalFine = table.Column<decimal>(type: "numeric", nullable: false),
                    ShippingAddress = table.Column<string>(type: "text", nullable: false),
                    TransactionId = table.Column<string>(type: "text", nullable: true),
                    AccountId1 = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Order_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Order_Account_AccountId1",
                        column: x => x.AccountId1,
                        principalTable: "Account",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Order_Transaction_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transaction",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Diamond",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    JewelryId = table.Column<string>(type: "text", nullable: true),
                    DiamondShapeId = table.Column<string>(type: "text", nullable: false),
                    Clarity = table.Column<string>(type: "text", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: false),
                    Cut = table.Column<string>(type: "text", nullable: true),
                    PriceOffset = table.Column<decimal>(type: "numeric", nullable: false),
                    Carat = table.Column<float>(type: "real", nullable: false),
                    HasGIACert = table.Column<bool>(type: "boolean", nullable: false),
                    IsLabDiamond = table.Column<bool>(type: "boolean", nullable: false),
                    WidthLengthRatio = table.Column<float>(type: "real", nullable: false),
                    Depth = table.Column<float>(type: "real", nullable: false),
                    Table = table.Column<float>(type: "real", nullable: false),
                    Polish = table.Column<string>(type: "text", nullable: false),
                    Symmetry = table.Column<string>(type: "text", nullable: false),
                    Girdle = table.Column<string>(type: "text", nullable: false),
                    Culet = table.Column<string>(type: "text", nullable: false),
                    Fluorescence = table.Column<string>(type: "text", nullable: false),
                    Measurement = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diamond", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Diamond_Diamond_Shape_DiamondShapeId",
                        column: x => x.DiamondShapeId,
                        principalTable: "Diamond_Shape",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Diamond_Jewelry_JewelryId",
                        column: x => x.JewelryId,
                        principalTable: "Jewelry",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JewelryReview",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    AccountId = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    StarRating = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false),
                    AccountId1 = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JewelryReview", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JewelryReview_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JewelryReview_Account_AccountId1",
                        column: x => x.AccountId1,
                        principalTable: "Account",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JewelryReview_Jewelry_Id",
                        column: x => x.Id,
                        principalTable: "Jewelry",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JewelrySideDiamond",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    JewelryId = table.Column<string>(type: "text", nullable: true),
                    Carat = table.Column<float>(type: "real", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    ColorMin = table.Column<string>(type: "text", nullable: false),
                    ColorMax = table.Column<string>(type: "text", nullable: false),
                    ClarityMin = table.Column<string>(type: "text", nullable: false),
                    ClarityMax = table.Column<int>(type: "integer", nullable: false),
                    SettingType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JewelrySideDiamond", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JewelrySideDiamond_Jewelry_JewelryId",
                        column: x => x.JewelryId,
                        principalTable: "Jewelry",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JewelryWarranty",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiredDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    WarrantyPath = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JewelryWarranty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JewelryWarranty_Jewelry_Id",
                        column: x => x.Id,
                        principalTable: "Jewelry",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MainDiamondShape",
                columns: table => new
                {
                    MainDiamondReqId = table.Column<string>(type: "text", nullable: false),
                    ShapeId = table.Column<string>(type: "text", nullable: false),
                    CaratFrom = table.Column<float>(type: "real", nullable: false),
                    CaratTo = table.Column<float>(type: "real", nullable: false),
                    MainDiamondReqId1 = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainDiamondShape", x => new { x.MainDiamondReqId, x.ShapeId });
                    table.ForeignKey(
                        name: "FK_MainDiamondShape_Diamond_Shape_ShapeId",
                        column: x => x.ShapeId,
                        principalTable: "Diamond_Shape",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MainDiamondShape_MainDiamondReq_MainDiamondReqId",
                        column: x => x.MainDiamondReqId,
                        principalTable: "MainDiamondReq",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MainDiamondShape_MainDiamondReq_MainDiamondReqId1",
                        column: x => x.MainDiamondReqId1,
                        principalTable: "MainDiamondReq",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PromoReqShape",
                columns: table => new
                {
                    PromoReqId = table.Column<string>(type: "text", nullable: false),
                    ShapeId = table.Column<string>(type: "text", nullable: false),
                    PromoReqId1 = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoReqShape", x => new { x.PromoReqId, x.ShapeId });
                    table.ForeignKey(
                        name: "FK_PromoReqShape_Diamond_Shape_ShapeId",
                        column: x => x.ShapeId,
                        principalTable: "Diamond_Shape",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromoReqShape_PromoReq_PromoReqId",
                        column: x => x.PromoReqId,
                        principalTable: "PromoReq",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromoReqShape_PromoReq_PromoReqId1",
                        column: x => x.PromoReqId1,
                        principalTable: "PromoReq",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SideDiamondOpt",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CaratWeight = table.Column<float>(type: "real", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    SideDiamondReqId = table.Column<string>(type: "text", nullable: false),
                    SideDiamondReqId1 = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SideDiamondOpt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SideDiamondOpt_SideDiamondReq_SideDiamondReqId",
                        column: x => x.SideDiamondReqId,
                        principalTable: "SideDiamondReq",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SideDiamondOpt_SideDiamondReq_SideDiamondReqId1",
                        column: x => x.SideDiamondReqId1,
                        principalTable: "SideDiamondReq",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    AccountId = table.Column<string>(type: "text", nullable: false),
                    OrderId = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notification_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notification_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderLog",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    OrderId = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PreviousLogId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderLog_OrderLog_PreviousLogId",
                        column: x => x.PreviousLogId,
                        principalTable: "OrderLog",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderLog_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiamondWarranty",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiredDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    WarrantyType = table.Column<int>(type: "integer", nullable: false),
                    WarrantyPath = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiamondWarranty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiamondWarranty_Diamond_Id",
                        column: x => x.Id,
                        principalTable: "Diamond",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderItem",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    OrderId = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    JewelryId = table.Column<string>(type: "text", nullable: true),
                    DiamondId = table.Column<string>(type: "text", nullable: true),
                    EngravedText = table.Column<string>(type: "text", nullable: false),
                    EngravedFont = table.Column<string>(type: "text", nullable: false),
                    PurchasedPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    DiscountCode = table.Column<string>(type: "text", nullable: false),
                    DiscountPercent = table.Column<int>(type: "integer", nullable: false),
                    PromoCode = table.Column<string>(type: "text", nullable: false),
                    PromoPercent = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItem_Diamond_DiamondId",
                        column: x => x.DiamondId,
                        principalTable: "Diamond",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderItem_Jewelry_JewelryId",
                        column: x => x.JewelryId,
                        principalTable: "Jewelry",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderItem_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Account_Role",
                columns: new[] { "Id", "RoleDescription", "RoleName", "RoleType" },
                values: new object[,]
                {
                    { "1", "customer", "customer", 0 },
                    { "11", "staff", "staff", 1 },
                    { "2", "customer_bronze", "customer_bronze", 0 },
                    { "22", "manager", "manager", 1 },
                    { "3", "customer_silver", "customer_silver", 0 },
                    { "33", "admin", "admin", 1 },
                    { "4", "customer_gold", "customer_gold", 0 }
                });

            migrationBuilder.InsertData(
                table: "Diamond_Shape",
                columns: new[] { "Id", "Shape" },
                values: new object[,]
                {
                    { "1", "Round" },
                    { "10", "Pear" },
                    { "2", "Princess" },
                    { "3", "Cushion" },
                    { "4", "Emerald" },
                    { "5", "Oval" },
                    { "6", "Radiant" },
                    { "7", "Asscher" },
                    { "8", "Marquise" },
                    { "9", "Heart" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_Id",
                table: "Account",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Account_IdentityId",
                table: "Account",
                column: "IdentityId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Account_Role_RoleName",
                table: "Account_Role",
                column: "RoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountAccountRole_RolesId",
                table: "AccountAccountRole",
                column: "RolesId");

            migrationBuilder.CreateIndex(
                name: "IX_Address_AccountId",
                table: "Address",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Address_Id_AccountId",
                table: "Address",
                columns: new[] { "Id", "AccountId" });

            migrationBuilder.CreateIndex(
                name: "IX_Blog_AccountId",
                table: "Blog",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Diamond_DiamondShapeId",
                table: "Diamond",
                column: "DiamondShapeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Diamond_Id",
                table: "Diamond",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Diamond_JewelryId",
                table: "Diamond",
                column: "JewelryId");

            migrationBuilder.CreateIndex(
                name: "IX_Diamond_Shape_Id",
                table: "Diamond_Shape",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DiamondPrice_CriteriaId",
                table: "DiamondPrice",
                column: "CriteriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Gift_PromotionId",
                table: "Gift",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_Jewelry_Id",
                table: "Jewelry",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Jewelry_MetalId",
                table: "Jewelry",
                column: "MetalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jewelry_ModelId",
                table: "Jewelry",
                column: "ModelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jewelry_SizeId",
                table: "Jewelry",
                column: "SizeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JewelryModel_CategoryId",
                table: "JewelryModel",
                column: "CategoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JewelryModel_Id",
                table: "JewelryModel",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_JewelryModelCategory_ParentCategoryId",
                table: "JewelryModelCategory",
                column: "ParentCategoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JewelryReview_AccountId",
                table: "JewelryReview",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_JewelryReview_AccountId1",
                table: "JewelryReview",
                column: "AccountId1");

            migrationBuilder.CreateIndex(
                name: "IX_JewelrySideDiamond_Id",
                table: "JewelrySideDiamond",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_JewelrySideDiamond_JewelryId",
                table: "JewelrySideDiamond",
                column: "JewelryId");

            migrationBuilder.CreateIndex(
                name: "IX_JewelryWarranty_Id",
                table: "JewelryWarranty",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_MainDiamondReq_ModelId",
                table: "MainDiamondReq",
                column: "ModelId");

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
                name: "IX_MainDiamondShape_ShapeId",
                table: "MainDiamondShape",
                column: "ShapeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notification_AccountId",
                table: "Notification",
                column: "AccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notification_OrderId",
                table: "Notification",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_AccountId",
                table: "Order",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_AccountId1",
                table: "Order",
                column: "AccountId1");

            migrationBuilder.CreateIndex(
                name: "IX_Order_TransactionId",
                table: "Order",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_DiamondId",
                table: "OrderItem",
                column: "DiamondId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_JewelryId",
                table: "OrderItem",
                column: "JewelryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_OrderId",
                table: "OrderItem",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLog_OrderId",
                table: "OrderLog",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLog_PreviousLogId",
                table: "OrderLog",
                column: "PreviousLogId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_outbox_message_Id",
                table: "outbox_message",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PromoReq_DiscountId",
                table: "PromoReq",
                column: "DiscountId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoReq_ModelId",
                table: "PromoReq",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoReq_PromotionId",
                table: "PromoReq",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoReqShape_PromoReqId1",
                table: "PromoReqShape",
                column: "PromoReqId1");

            migrationBuilder.CreateIndex(
                name: "IX_PromoReqShape_ShapeId",
                table: "PromoReqShape",
                column: "ShapeId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Role",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_SideDiamondOpt_SideDiamondReqId",
                table: "SideDiamondOpt",
                column: "SideDiamondReqId");

            migrationBuilder.CreateIndex(
                name: "IX_SideDiamondOpt_SideDiamondReqId1",
                table: "SideDiamondOpt",
                column: "SideDiamondReqId1");

            migrationBuilder.CreateIndex(
                name: "IX_SideDiamondReq_ModelId",
                table: "SideDiamondReq",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_SideDiamondReq_ModelId1",
                table: "SideDiamondReq",
                column: "ModelId1");

            migrationBuilder.CreateIndex(
                name: "IX_SideDiamondReq_ShapeId",
                table: "SideDiamondReq",
                column: "ShapeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SizeMetal_JewelryModelId",
                table: "SizeMetal",
                column: "JewelryModelId");

            migrationBuilder.CreateIndex(
                name: "IX_SizeMetal_MetalId",
                table: "SizeMetal",
                column: "MetalId");

            migrationBuilder.CreateIndex(
                name: "IX_SizeMetal_ModelId",
                table: "SizeMetal",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_PayMethodId",
                table: "Transaction",
                column: "PayMethodId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_RefundedTransacId",
                table: "Transaction",
                column: "RefundedTransacId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "User",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "User",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountAccountRole");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "Blog");

            migrationBuilder.DropTable(
                name: "DiamondPrice");

            migrationBuilder.DropTable(
                name: "DiamondWarranty");

            migrationBuilder.DropTable(
                name: "Gift");

            migrationBuilder.DropTable(
                name: "JewelryReview");

            migrationBuilder.DropTable(
                name: "JewelrySideDiamond");

            migrationBuilder.DropTable(
                name: "JewelryWarranty");

            migrationBuilder.DropTable(
                name: "MainDiamondShape");

            migrationBuilder.DropTable(
                name: "News");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "OrderItem");

            migrationBuilder.DropTable(
                name: "OrderLog");

            migrationBuilder.DropTable(
                name: "outbox_message");

            migrationBuilder.DropTable(
                name: "PromoReqShape");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "SideDiamondOpt");

            migrationBuilder.DropTable(
                name: "SizeMetal");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserToken");

            migrationBuilder.DropTable(
                name: "Account_Role");

            migrationBuilder.DropTable(
                name: "DiamondCriteria");

            migrationBuilder.DropTable(
                name: "MainDiamondReq");

            migrationBuilder.DropTable(
                name: "Diamond");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "PromoReq");

            migrationBuilder.DropTable(
                name: "SideDiamondReq");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Jewelry");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "Transaction");

            migrationBuilder.DropTable(
                name: "Discount");

            migrationBuilder.DropTable(
                name: "Promotion");

            migrationBuilder.DropTable(
                name: "Diamond_Shape");

            migrationBuilder.DropTable(
                name: "JewelryModel");

            migrationBuilder.DropTable(
                name: "Metal");

            migrationBuilder.DropTable(
                name: "Size");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "PaymentMethod");

            migrationBuilder.DropTable(
                name: "JewelryModelCategory");
        }
    }
}
