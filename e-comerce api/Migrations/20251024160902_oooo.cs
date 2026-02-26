using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace e_comerce_api.Migrations
{
    /// <inheritdoc />
    public partial class oooo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttributeValues_ProductAttributies_ProductAttributiesProductAttriid",
                table: "ProductAttributeValues");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttributeValues_Products_ProductId",
                table: "ProductAttributeValues");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttributies_Products_product_id",
                table: "ProductAttributies");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttributies_SubCategories_SubCategorySbuCatId",
                table: "ProductAttributies");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_Products_product_id",
                table: "ProductVariants");

            migrationBuilder.DropForeignKey(
                name: "FK_VariantAttributes_ProductVariants_ProductVariantId",
                table: "VariantAttributes");

            migrationBuilder.DropIndex(
                name: "IX_VariantAttributes_ProductVariantId",
                table: "VariantAttributes");

            migrationBuilder.DropIndex(
                name: "IX_ProductAttributies_product_id",
                table: "ProductAttributies");

            migrationBuilder.DropIndex(
                name: "IX_ProductAttributies_SubCategorySbuCatId",
                table: "ProductAttributies");

            migrationBuilder.DropIndex(
                name: "IX_ProductAttributeValues_ProductAttributiesProductAttriid",
                table: "ProductAttributeValues");

            migrationBuilder.DropColumn(
                name: "ProductVariantId",
                table: "VariantAttributes");

            migrationBuilder.DropColumn(
                name: "AverageRate",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "SubCategorySbuCatId",
                table: "ProductAttributies");

            migrationBuilder.DropColumn(
                name: "product_id",
                table: "ProductAttributies");

            migrationBuilder.DropColumn(
                name: "ProductAttributiesProductAttriid",
                table: "ProductAttributeValues");

            migrationBuilder.AddColumn<string>(
                name: "possiblevalue",
                table: "VariantAttributes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "product_id",
                table: "ProductVariants",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "discountpercentage",
                table: "ProductVariants",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FinalPrice",
                table: "ProductVariants",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "SKU",
                table: "ProductVariants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<bool>(
                name: "isaveliable",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "discountpercentage",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "AverageRate",
                table: "Products",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FinalPrice",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "subcatid",
                table: "ProductAttributies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductAttributeValues",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AttributeId",
                table: "ProductAttributeValues",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "AffiliateComession",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductImages",
                columns: table => new
                {
                    ImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImages", x => x.ImageId);
                    table.ForeignKey(
                        name: "FK_ProductImages_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VariantAttributes_variatn_id",
                table: "VariantAttributes",
                column: "variatn_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributies_subcatid",
                table: "ProductAttributies",
                column: "subcatid");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeValues_AttributeId",
                table: "ProductAttributeValues",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_AffiliateComession_ProductId",
                table: "AffiliateComession",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId",
                table: "ProductImages",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_AffiliateComession_Products_ProductId",
                table: "AffiliateComession",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttributeValues_ProductAttributies_AttributeId",
                table: "ProductAttributeValues",
                column: "AttributeId",
                principalTable: "ProductAttributies",
                principalColumn: "ProductAttriid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttributeValues_Products_ProductId",
                table: "ProductAttributeValues",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttributies_SubCategories_subcatid",
                table: "ProductAttributies",
                column: "subcatid",
                principalTable: "SubCategories",
                principalColumn: "SbuCatId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_Products_product_id",
                table: "ProductVariants",
                column: "product_id",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VariantAttributes_ProductVariants_variatn_id",
                table: "VariantAttributes",
                column: "variatn_id",
                principalTable: "ProductVariants",
                principalColumn: "ProductVariantId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AffiliateComession_Products_ProductId",
                table: "AffiliateComession");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttributeValues_ProductAttributies_AttributeId",
                table: "ProductAttributeValues");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttributeValues_Products_ProductId",
                table: "ProductAttributeValues");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttributies_SubCategories_subcatid",
                table: "ProductAttributies");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_Products_product_id",
                table: "ProductVariants");

            migrationBuilder.DropForeignKey(
                name: "FK_VariantAttributes_ProductVariants_variatn_id",
                table: "VariantAttributes");

            migrationBuilder.DropTable(
                name: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_VariantAttributes_variatn_id",
                table: "VariantAttributes");

            migrationBuilder.DropIndex(
                name: "IX_ProductAttributies_subcatid",
                table: "ProductAttributies");

            migrationBuilder.DropIndex(
                name: "IX_ProductAttributeValues_AttributeId",
                table: "ProductAttributeValues");

            migrationBuilder.DropIndex(
                name: "IX_AffiliateComession_ProductId",
                table: "AffiliateComession");

            migrationBuilder.DropColumn(
                name: "possiblevalue",
                table: "VariantAttributes");

            migrationBuilder.DropColumn(
                name: "FinalPrice",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "SKU",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "FinalPrice",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "subcatid",
                table: "ProductAttributies");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "AffiliateComession");

            migrationBuilder.AddColumn<int>(
                name: "ProductVariantId",
                table: "VariantAttributes",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "product_id",
                table: "ProductVariants",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "discountpercentage",
                table: "ProductVariants",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<double>(
                name: "AverageRate",
                table: "ProductVariants",
                type: "float",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "isaveliable",
                table: "Products",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<decimal>(
                name: "discountpercentage",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "AverageRate",
                table: "Products",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<int>(
                name: "SubCategorySbuCatId",
                table: "ProductAttributies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "product_id",
                table: "ProductAttributies",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductAttributeValues",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AttributeId",
                table: "ProductAttributeValues",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ProductAttributiesProductAttriid",
                table: "ProductAttributeValues",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VariantAttributes_ProductVariantId",
                table: "VariantAttributes",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributies_product_id",
                table: "ProductAttributies",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributies_SubCategorySbuCatId",
                table: "ProductAttributies",
                column: "SubCategorySbuCatId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeValues_ProductAttributiesProductAttriid",
                table: "ProductAttributeValues",
                column: "ProductAttributiesProductAttriid");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttributeValues_ProductAttributies_ProductAttributiesProductAttriid",
                table: "ProductAttributeValues",
                column: "ProductAttributiesProductAttriid",
                principalTable: "ProductAttributies",
                principalColumn: "ProductAttriid");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttributeValues_Products_ProductId",
                table: "ProductAttributeValues",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttributies_Products_product_id",
                table: "ProductAttributies",
                column: "product_id",
                principalTable: "Products",
                principalColumn: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttributies_SubCategories_SubCategorySbuCatId",
                table: "ProductAttributies",
                column: "SubCategorySbuCatId",
                principalTable: "SubCategories",
                principalColumn: "SbuCatId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_Products_product_id",
                table: "ProductVariants",
                column: "product_id",
                principalTable: "Products",
                principalColumn: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_VariantAttributes_ProductVariants_ProductVariantId",
                table: "VariantAttributes",
                column: "ProductVariantId",
                principalTable: "ProductVariants",
                principalColumn: "ProductVariantId");
        }
    }
}
