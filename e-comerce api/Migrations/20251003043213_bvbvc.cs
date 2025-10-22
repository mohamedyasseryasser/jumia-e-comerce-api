using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace e_comerce_api.Migrations
{
    /// <inheritdoc />
    public partial class bvbvc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_addresses_AspNetUsers_UserId",
                table: "addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_admins_AspNetUsers_user_id",
                table: "admins");

            migrationBuilder.DropForeignKey(
                name: "FK_customers_AspNetUsers_user_id",
                table: "customers");

            migrationBuilder.DropForeignKey(
                name: "FK_sellers_AspNetUsers_user_id",
                table: "sellers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_sellers",
                table: "sellers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_customers",
                table: "customers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_admins",
                table: "admins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_addresses",
                table: "addresses");

            migrationBuilder.RenameTable(
                name: "sellers",
                newName: "Sellers");

            migrationBuilder.RenameTable(
                name: "customers",
                newName: "Customers");

            migrationBuilder.RenameTable(
                name: "admins",
                newName: "Admins");

            migrationBuilder.RenameTable(
                name: "addresses",
                newName: "Addresses");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Sellers",
                newName: "Sellerid");

            migrationBuilder.RenameIndex(
                name: "IX_sellers_user_id",
                table: "Sellers",
                newName: "IX_Sellers_user_id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Customers",
                newName: "Customerid");

            migrationBuilder.RenameIndex(
                name: "IX_customers_user_id",
                table: "Customers",
                newName: "IX_Customers_user_id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Admins",
                newName: "Adminid");

            migrationBuilder.RenameIndex(
                name: "IX_admins_user_id",
                table: "Admins",
                newName: "IX_Admins_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_addresses_UserId",
                table: "Addresses",
                newName: "IX_Addresses_UserId");

            migrationBuilder.AddColumn<string>(
                name: "AddressName",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sellers",
                table: "Sellers",
                column: "Sellerid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customers",
                table: "Customers",
                column: "Customerid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Admins",
                table: "Admins",
                column: "Adminid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Addresses",
                table: "Addresses",
                column: "AddressId");

            migrationBuilder.CreateTable(
                name: "Affiliates",
                columns: table => new
                {
                    AffiliateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AffiliateCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommissionRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalEarnings = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AvailableBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    WithdrawnAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Affiliates", x => x.AffiliateId);
                    table.ForeignKey(
                        name: "FK_Affiliates_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    Cartid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    totalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    customer_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.Cartid);
                    table.ForeignKey(
                        name: "FK_Carts_Customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "Customers",
                        principalColumn: "Customerid");
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    images = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    CoponId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    decription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    startAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    endAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsageLimit = table.Column<int>(type: "int", nullable: true),
                    UsageCount = table.Column<int>(type: "int", nullable: true),
                    MinimumPurchase = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.CoponId);
                });

            migrationBuilder.CreateTable(
                name: "Wishlists",
                columns: table => new
                {
                    WishlistId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wishlists", x => x.WishlistId);
                    table.ForeignKey(
                        name: "FK_Wishlists_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Customerid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AffiliateSellerRelationships",
                columns: table => new
                {
                    RelationshipId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AffiliateId = table.Column<int>(type: "int", nullable: false),
                    SellerId = table.Column<int>(type: "int", nullable: false),
                    CommissionRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AffiliateSellerRelationships", x => x.RelationshipId);
                    table.ForeignKey(
                        name: "FK_AffiliateSellerRelationships_Affiliates_AffiliateId",
                        column: x => x.AffiliateId,
                        principalTable: "Affiliates",
                        principalColumn: "AffiliateId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AffiliateSellerRelationships_Sellers_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Sellers",
                        principalColumn: "Sellerid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AffiliateWithDrawel",
                columns: table => new
                {
                    WithdrawalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AffiliateId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AffiliateWithDrawel", x => x.WithdrawalId);
                    table.ForeignKey(
                        name: "FK_AffiliateWithDrawel_Affiliates_AffiliateId",
                        column: x => x.AffiliateId,
                        principalTable: "Affiliates",
                        principalColumn: "AffiliateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubCategories",
                columns: table => new
                {
                    SbuCatId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    cat_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCategories", x => x.SbuCatId);
                    table.ForeignKey(
                        name: "FK_SubCategories_Categories_cat_id",
                        column: x => x.cat_id,
                        principalTable: "Categories",
                        principalColumn: "CategoryId");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Orderid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ShippingFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FinalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    statue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    address_id = table.Column<int>(type: "int", nullable: true),
                    customer_id = table.Column<int>(type: "int", nullable: true),
                    AffiliateId = table.Column<int>(type: "int", nullable: true),
                    CoponsCoponId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Orderid);
                    table.ForeignKey(
                        name: "FK_Orders_Addresses_address_id",
                        column: x => x.address_id,
                        principalTable: "Addresses",
                        principalColumn: "AddressId");
                    table.ForeignKey(
                        name: "FK_Orders_Affiliates_AffiliateId",
                        column: x => x.AffiliateId,
                        principalTable: "Affiliates",
                        principalColumn: "AffiliateId");
                    table.ForeignKey(
                        name: "FK_Orders_Coupons_CoponsCoponId",
                        column: x => x.CoponsCoponId,
                        principalTable: "Coupons",
                        principalColumn: "CoponId");
                    table.ForeignKey(
                        name: "FK_Orders_Customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "Customers",
                        principalColumn: "Customerid");
                });

            migrationBuilder.CreateTable(
                name: "UserCoupons",
                columns: table => new
                {
                    UserCoponId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsUsed = table.Column<bool>(type: "bit", nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    copon_id = table.Column<int>(type: "int", nullable: false),
                    customer_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCoupons", x => x.UserCoponId);
                    table.ForeignKey(
                        name: "FK_UserCoupons_Coupons_copon_id",
                        column: x => x.copon_id,
                        principalTable: "Coupons",
                        principalColumn: "CoponId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCoupons_Customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "Customers",
                        principalColumn: "Customerid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mainimageurl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    basicprice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    discountpercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    stockquantity = table.Column<int>(type: "int", nullable: false),
                    AverageRate = table.Column<double>(type: "float", nullable: true),
                    isaveliable = table.Column<bool>(type: "bit", nullable: true),
                    seller_id = table.Column<int>(type: "int", nullable: true),
                    sub_cat_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Products_Sellers_seller_id",
                        column: x => x.seller_id,
                        principalTable: "Sellers",
                        principalColumn: "Sellerid");
                    table.ForeignKey(
                        name: "FK_Products_SubCategories_sub_cat_id",
                        column: x => x.sub_cat_id,
                        principalTable: "SubCategories",
                        principalColumn: "SbuCatId");
                });

            migrationBuilder.CreateTable(
                name: "AffiliateComession",
                columns: table => new
                {
                    CommissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AffiliateId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    OrderAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CommissionRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CommissionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AffiliateComession", x => x.CommissionId);
                    table.ForeignKey(
                        name: "FK_AffiliateComession_Affiliates_AffiliateId",
                        column: x => x.AffiliateId,
                        principalTable: "Affiliates",
                        principalColumn: "AffiliateId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AffiliateComession_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Orderid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubOrders",
                columns: table => new
                {
                    SubOrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrackingNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShippingProvider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    seller_id = table.Column<int>(type: "int", nullable: true),
                    OrderId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubOrders", x => x.SubOrderId);
                    table.ForeignKey(
                        name: "FK_SubOrders_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Orderid");
                    table.ForeignKey(
                        name: "FK_SubOrders_Sellers_seller_id",
                        column: x => x.seller_id,
                        principalTable: "Sellers",
                        principalColumn: "Sellerid");
                });

            migrationBuilder.CreateTable(
                name: "ProductAttributies",
                columns: table => new
                {
                    ProductAttriid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttributeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PossibleValues = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: true),
                    IsFilterable = table.Column<bool>(type: "bit", nullable: true),
                    product_id = table.Column<int>(type: "int", nullable: true),
                    SubCategorySbuCatId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttributies", x => x.ProductAttriid);
                    table.ForeignKey(
                        name: "FK_ProductAttributies_Products_product_id",
                        column: x => x.product_id,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                    table.ForeignKey(
                        name: "FK_ProductAttributies_SubCategories_SubCategorySbuCatId",
                        column: x => x.SubCategorySbuCatId,
                        principalTable: "SubCategories",
                        principalColumn: "SbuCatId");
                });

            migrationBuilder.CreateTable(
                name: "ProductVariants",
                columns: table => new
                {
                    ProductVariantId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VariantName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    basicprice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mainimageurl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    stockquantity = table.Column<int>(type: "int", nullable: false),
                    discountpercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AverageRate = table.Column<double>(type: "float", nullable: true),
                    isdefault = table.Column<bool>(type: "bit", nullable: true),
                    isaveliable = table.Column<bool>(type: "bit", nullable: true),
                    product_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariants", x => x.ProductVariantId);
                    table.ForeignKey(
                        name: "FK_ProductVariants_Products_product_id",
                        column: x => x.product_id,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                });

            migrationBuilder.CreateTable(
                name: "ProductViews",
                columns: table => new
                {
                    ProductViewid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ViewedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductViews", x => x.ProductViewid);
                    table.ForeignKey(
                        name: "FK_ProductViews_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Customerid");
                    table.ForeignKey(
                        name: "FK_ProductViews_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    RateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    rate = table.Column<int>(type: "int", nullable: false),
                    IsVerifiedPurchase = table.Column<bool>(type: "bit", nullable: true),
                    HelpfulCount = table.Column<int>(type: "int", nullable: true),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    productid = table.Column<int>(type: "int", nullable: false),
                    customerid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.RateId);
                    table.ForeignKey(
                        name: "FK_Ratings_Customers_customerid",
                        column: x => x.customerid,
                        principalTable: "Customers",
                        principalColumn: "Customerid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ratings_Products_productid",
                        column: x => x.productid,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WishlistItems",
                columns: table => new
                {
                    WishlistItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WishlistId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WishlistItems", x => x.WishlistItemId);
                    table.ForeignKey(
                        name: "FK_WishlistItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WishlistItems_Wishlists_WishlistId",
                        column: x => x.WishlistId,
                        principalTable: "Wishlists",
                        principalColumn: "WishlistId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductAttributeValues",
                columns: table => new
                {
                    ProductAttrValueId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    AttributeId = table.Column<int>(type: "int", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductAttributiesProductAttriid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttributeValues", x => x.ProductAttrValueId);
                    table.ForeignKey(
                        name: "FK_ProductAttributeValues_ProductAttributies_ProductAttributiesProductAttriid",
                        column: x => x.ProductAttributiesProductAttriid,
                        principalTable: "ProductAttributies",
                        principalColumn: "ProductAttriid");
                    table.ForeignKey(
                        name: "FK_ProductAttributeValues_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    CartItemsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CartId = table.Column<int>(type: "int", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    PriceAtAddition = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VariantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.CartItemsId);
                    table.ForeignKey(
                        name: "FK_CartItems_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "Cartid");
                    table.ForeignKey(
                        name: "FK_CartItems_ProductVariants_VariantId",
                        column: x => x.VariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "ProductVariantId");
                    table.ForeignKey(
                        name: "FK_CartItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    OrderItemsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    PriceAtPurchase = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: true),
                    sub_order_id = table.Column<int>(type: "int", nullable: true),
                    variant_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.OrderItemsId);
                    table.ForeignKey(
                        name: "FK_OrderItems_ProductVariants_variant_id",
                        column: x => x.variant_id,
                        principalTable: "ProductVariants",
                        principalColumn: "ProductVariantId");
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_product_id",
                        column: x => x.product_id,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                    table.ForeignKey(
                        name: "FK_OrderItems_SubOrders_sub_order_id",
                        column: x => x.sub_order_id,
                        principalTable: "SubOrders",
                        principalColumn: "SubOrderId");
                });

            migrationBuilder.CreateTable(
                name: "VariantAttributes",
                columns: table => new
                {
                    ProductVariantAttrid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttributeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttributeValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    variatn_id = table.Column<int>(type: "int", nullable: true),
                    ProductVariantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VariantAttributes", x => x.ProductVariantAttrid);
                    table.ForeignKey(
                        name: "FK_VariantAttributes_ProductVariants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "ProductVariantId");
                });

            migrationBuilder.CreateTable(
                name: "HelpfulRatings",
                columns: table => new
                {
                    HelpfulId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RatingId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    IsHelpful = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HelpfulRatings", x => x.HelpfulId);
                    table.ForeignKey(
                        name: "FK_HelpfulRatings_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Customerid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HelpfulRatings_Ratings_RatingId",
                        column: x => x.RatingId,
                        principalTable: "Ratings",
                        principalColumn: "RateId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReviewImages",
                columns: table => new
                {
                    ReviewImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RatingId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewImages", x => x.ReviewImageId);
                    table.ForeignKey(
                        name: "FK_ReviewImages_Ratings_RatingId",
                        column: x => x.RatingId,
                        principalTable: "Ratings",
                        principalColumn: "RateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AffiliateComession_AffiliateId",
                table: "AffiliateComession",
                column: "AffiliateId");

            migrationBuilder.CreateIndex(
                name: "IX_AffiliateComession_OrderId",
                table: "AffiliateComession",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Affiliates_UserId",
                table: "Affiliates",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AffiliateSellerRelationships_AffiliateId",
                table: "AffiliateSellerRelationships",
                column: "AffiliateId");

            migrationBuilder.CreateIndex(
                name: "IX_AffiliateSellerRelationships_SellerId",
                table: "AffiliateSellerRelationships",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_AffiliateWithDrawel_AffiliateId",
                table: "AffiliateWithDrawel",
                column: "AffiliateId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId",
                table: "CartItems",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductId",
                table: "CartItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_VariantId",
                table: "CartItems",
                column: "VariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_customer_id",
                table: "Carts",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_HelpfulRatings_CustomerId",
                table: "HelpfulRatings",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_HelpfulRatings_RatingId",
                table: "HelpfulRatings",
                column: "RatingId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_product_id",
                table: "OrderItems",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_sub_order_id",
                table: "OrderItems",
                column: "sub_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_variant_id",
                table: "OrderItems",
                column: "variant_id");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_address_id",
                table: "Orders",
                column: "address_id");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_AffiliateId",
                table: "Orders",
                column: "AffiliateId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CoponsCoponId",
                table: "Orders",
                column: "CoponsCoponId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_customer_id",
                table: "Orders",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeValues_ProductAttributiesProductAttriid",
                table: "ProductAttributeValues",
                column: "ProductAttributiesProductAttriid");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeValues_ProductId",
                table: "ProductAttributeValues",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributies_product_id",
                table: "ProductAttributies",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributies_SubCategorySbuCatId",
                table: "ProductAttributies",
                column: "SubCategorySbuCatId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_seller_id",
                table: "Products",
                column: "seller_id");

            migrationBuilder.CreateIndex(
                name: "IX_Products_sub_cat_id",
                table: "Products",
                column: "sub_cat_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_product_id",
                table: "ProductVariants",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductViews_CustomerId",
                table: "ProductViews",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductViews_ProductId",
                table: "ProductViews",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_customerid",
                table: "Ratings",
                column: "customerid");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_productid",
                table: "Ratings",
                column: "productid");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewImages_RatingId",
                table: "ReviewImages",
                column: "RatingId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategories_cat_id",
                table: "SubCategories",
                column: "cat_id");

            migrationBuilder.CreateIndex(
                name: "IX_SubOrders_OrderId",
                table: "SubOrders",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SubOrders_seller_id",
                table: "SubOrders",
                column: "seller_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserCoupons_copon_id",
                table: "UserCoupons",
                column: "copon_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserCoupons_customer_id",
                table: "UserCoupons",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_VariantAttributes_ProductVariantId",
                table: "VariantAttributes",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_WishlistItems_ProductId",
                table: "WishlistItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WishlistItems_WishlistId",
                table: "WishlistItems",
                column: "WishlistId");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_CustomerId",
                table: "Wishlists",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_AspNetUsers_UserId",
                table: "Addresses",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Admins_AspNetUsers_user_id",
                table: "Admins",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_AspNetUsers_user_id",
                table: "Customers",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sellers_AspNetUsers_user_id",
                table: "Sellers",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_AspNetUsers_UserId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Admins_AspNetUsers_user_id",
                table: "Admins");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_AspNetUsers_user_id",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Sellers_AspNetUsers_user_id",
                table: "Sellers");

            migrationBuilder.DropTable(
                name: "AffiliateComession");

            migrationBuilder.DropTable(
                name: "AffiliateSellerRelationships");

            migrationBuilder.DropTable(
                name: "AffiliateWithDrawel");

            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "HelpfulRatings");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "ProductAttributeValues");

            migrationBuilder.DropTable(
                name: "ProductViews");

            migrationBuilder.DropTable(
                name: "ReviewImages");

            migrationBuilder.DropTable(
                name: "UserCoupons");

            migrationBuilder.DropTable(
                name: "VariantAttributes");

            migrationBuilder.DropTable(
                name: "WishlistItems");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "SubOrders");

            migrationBuilder.DropTable(
                name: "ProductAttributies");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropTable(
                name: "ProductVariants");

            migrationBuilder.DropTable(
                name: "Wishlists");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Affiliates");

            migrationBuilder.DropTable(
                name: "Coupons");

            migrationBuilder.DropTable(
                name: "SubCategories");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sellers",
                table: "Sellers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Customers",
                table: "Customers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Admins",
                table: "Admins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Addresses",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "AddressName",
                table: "Addresses");

            migrationBuilder.RenameTable(
                name: "Sellers",
                newName: "sellers");

            migrationBuilder.RenameTable(
                name: "Customers",
                newName: "customers");

            migrationBuilder.RenameTable(
                name: "Admins",
                newName: "admins");

            migrationBuilder.RenameTable(
                name: "Addresses",
                newName: "addresses");

            migrationBuilder.RenameColumn(
                name: "Sellerid",
                table: "sellers",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_Sellers_user_id",
                table: "sellers",
                newName: "IX_sellers_user_id");

            migrationBuilder.RenameColumn(
                name: "Customerid",
                table: "customers",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_Customers_user_id",
                table: "customers",
                newName: "IX_customers_user_id");

            migrationBuilder.RenameColumn(
                name: "Adminid",
                table: "admins",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_Admins_user_id",
                table: "admins",
                newName: "IX_admins_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_Addresses_UserId",
                table: "addresses",
                newName: "IX_addresses_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_sellers",
                table: "sellers",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_customers",
                table: "customers",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_admins",
                table: "admins",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_addresses",
                table: "addresses",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_addresses_AspNetUsers_UserId",
                table: "addresses",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_admins_AspNetUsers_user_id",
                table: "admins",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_customers_AspNetUsers_user_id",
                table: "customers",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_sellers_AspNetUsers_user_id",
                table: "sellers",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
