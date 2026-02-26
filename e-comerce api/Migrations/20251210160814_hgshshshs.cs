using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace e_comerce_api.Migrations
{
    /// <inheritdoc />
    public partial class hgshshshs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Coupons_CoponsCoponId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "CoponsCoponId",
                table: "Orders",
                newName: "CouponId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_CoponsCoponId",
                table: "Orders",
                newName: "IX_Orders_CouponId");

            migrationBuilder.AddColumn<string>(
                name: "AffiliateCode",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Coupons_CouponId",
                table: "Orders",
                column: "CouponId",
                principalTable: "Coupons",
                principalColumn: "CoponId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Coupons_CouponId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "AffiliateCode",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "CouponId",
                table: "Orders",
                newName: "CoponsCoponId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_CouponId",
                table: "Orders",
                newName: "IX_Orders_CoponsCoponId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Coupons_CoponsCoponId",
                table: "Orders",
                column: "CoponsCoponId",
                principalTable: "Coupons",
                principalColumn: "CoponId");
        }
    }
}
