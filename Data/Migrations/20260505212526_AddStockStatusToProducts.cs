using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechVault.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStockStatusToProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StockStatus",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "InStock");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockStatus",
                table: "Products");
        }
    }
}
