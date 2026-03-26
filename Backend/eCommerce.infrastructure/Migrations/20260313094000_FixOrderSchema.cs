using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCommerce.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixOrderSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add new required columns
            migrationBuilder.AddColumn<decimal>(
                name: "Subtotal",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Tax",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Shipping",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "BillingAddress",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StripePaymentIntentId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeSessionId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            // Copy TotalAmount to Total for existing records
            migrationBuilder.Sql("UPDATE Orders SET Total = TotalAmount WHERE TotalAmount IS NOT NULL");

            // Make ShippingAddress NOT NULL
            migrationBuilder.Sql("UPDATE Orders SET ShippingAddress = '' WHERE ShippingAddress IS NULL");
            
            migrationBuilder.AlterColumn<string>(
                name: "ShippingAddress",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            // Drop old columns
            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Orders");

            // Drop Notes column if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Orders' AND COLUMN_NAME = 'Notes')
                BEGIN
                    ALTER TABLE Orders DROP COLUMN Notes;
                END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Add back old columns
            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            // Copy Total back to TotalAmount
            migrationBuilder.Sql("UPDATE Orders SET TotalAmount = Total WHERE Total IS NOT NULL");

            // Make ShippingAddress nullable again
            migrationBuilder.AlterColumn<string>(
                name: "ShippingAddress",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // Drop new columns
            migrationBuilder.DropColumn(name: "Subtotal", table: "Orders");
            migrationBuilder.DropColumn(name: "Tax", table: "Orders");
            migrationBuilder.DropColumn(name: "Shipping", table: "Orders");
            migrationBuilder.DropColumn(name: "Total", table: "Orders");
            migrationBuilder.DropColumn(name: "BillingAddress", table: "Orders");
            migrationBuilder.DropColumn(name: "StripePaymentIntentId", table: "Orders");
            migrationBuilder.DropColumn(name: "StripeSessionId", table: "Orders");
        }
    }
}