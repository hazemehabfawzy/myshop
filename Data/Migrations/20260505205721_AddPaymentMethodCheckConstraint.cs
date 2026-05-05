using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechVault.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentMethodCheckConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Orders SET PaymentMethod = 'CashOnDelivery';");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Order_PaymentMethod",
                table: "Orders",
                sql: "PaymentMethod = 'CashOnDelivery'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Order_PaymentMethod",
                table: "Orders");
        }
    }
}
