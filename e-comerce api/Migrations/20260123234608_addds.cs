using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace e_comerce_api.Migrations
{
    /// <inheritdoc />
    public partial class addds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DiscountType",
                table: "Coupons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "OrderItemsId",
                table: "AffiliateComession",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AffiliateComession_OrderItemsId",
                table: "AffiliateComession",
                column: "OrderItemsId");

            migrationBuilder.AddForeignKey(
                name: "FK_AffiliateComession_OrderItems_OrderItemsId",
                table: "AffiliateComession",
                column: "OrderItemsId",
                principalTable: "OrderItems",
                principalColumn: "OrderItemsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AffiliateComession_OrderItems_OrderItemsId",
                table: "AffiliateComession");

            migrationBuilder.DropIndex(
                name: "IX_AffiliateComession_OrderItemsId",
                table: "AffiliateComession");

            migrationBuilder.DropColumn(
                name: "DiscountType",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "OrderItemsId",
                table: "AffiliateComession");
        }
    }
}
