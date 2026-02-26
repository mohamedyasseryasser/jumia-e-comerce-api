using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace e_comerce_api.Migrations
{
    /// <inheritdoc />
    public partial class hghghgh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "statue",
                table: "Carts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "statue",
                table: "Carts");
        }
    }
}
