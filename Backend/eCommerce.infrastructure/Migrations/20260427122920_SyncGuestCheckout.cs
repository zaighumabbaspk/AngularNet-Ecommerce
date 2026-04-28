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
                keyValue: "7d73e2d9-befd-4bbc-8b20-5106fb4e419f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fce134e1-425c-4ea8-a51a-95cf9fa1c805");

            // Only make UserId nullable - guest columns already exist
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            // Update existing guest columns to have proper constraints
            migrationBuilder.AlterColumn<string>(
                name: "GuestEmail",
                table: "Orders",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GuestOrderToken",
                table: "Orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "cd4b9e90-48fc-4262-b8f9-1f9936737876", null, "User", "USER" },
                    { "ea648fdb-1609-4fba-b5fc-5bd72d5c19b1", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd4b9e90-48fc-4262-b8f9-1f9936737876");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ea648fdb-1609-4fba-b5fc-5bd72d5c19b1");

            migrationBuilder.DropColumn(
                name: "GuestEmail",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "GuestOrderToken",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsGuestOrder",
                table: "Orders");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "7d73e2d9-befd-4bbc-8b20-5106fb4e419f", null, "User", "USER" },
                    { "fce134e1-425c-4ea8-a51a-95cf9fa1c805", null, "Admin", "ADMIN" }
                });
        }
    }
}
