using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace eCommerce.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncGuestCheckout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                    { "1a8b78f5-9238-4c92-a61e-21b35fbbe81a", null, "Admin", "ADMIN" },
                    { "d447b6fb-56cc-45e5-89df-6211634ecd1a", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1a8b78f5-9238-4c92-a61e-21b35fbbe81a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d447b6fb-56cc-45e5-89df-6211634ecd1a");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "71aec833-6cff-4e3e-b82b-a68d57cce398", null, "Admin", "ADMIN" },
                    { "a0692544-6bba-4fc5-a3c2-290e6e785429", null, "User", "USER" }
                });
        }
    }
}
