using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace e_comerce_api.Migrations
{
    /// <inheritdoc />
    public partial class adddd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Sellerid",
                table: "AffiliateComession",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AffiliateComession_Sellerid",
                table: "AffiliateComession",
                column: "Sellerid");

            migrationBuilder.AddForeignKey(
                name: "FK_AffiliateComession_Sellers_Sellerid",
                table: "AffiliateComession",
                column: "Sellerid",
                principalTable: "Sellers",
                principalColumn: "Sellerid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AffiliateComession_Sellers_Sellerid",
                table: "AffiliateComession");

            migrationBuilder.DropIndex(
                name: "IX_AffiliateComession_Sellerid",
                table: "AffiliateComession");

            migrationBuilder.DropColumn(
                name: "Sellerid",
                table: "AffiliateComession");
        }
    }
}
