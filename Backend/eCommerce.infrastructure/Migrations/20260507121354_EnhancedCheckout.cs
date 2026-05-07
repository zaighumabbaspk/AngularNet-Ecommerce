using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace eCommerce.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnhancedCheckout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "26d300da-073c-4a8e-973b-f1a2f8c3f0be");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "68f2fad9-f402-4586-9c14-16f4c22619f5");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "71aec833-6cff-4e3e-b82b-a68d57cce398", null, "Admin", "ADMIN" },
                    { "a0692544-6bba-4fc5-a3c2-290e6e785429", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "71aec833-6cff-4e3e-b82b-a68d57cce398");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a0692544-6bba-4fc5-a3c2-290e6e785429");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "26d300da-073c-4a8e-973b-f1a2f8c3f0be", null, "User", "USER" },
                    { "68f2fad9-f402-4586-9c14-16f4c22619f5", null, "Admin", "ADMIN" }
                });
        }
    }
}
