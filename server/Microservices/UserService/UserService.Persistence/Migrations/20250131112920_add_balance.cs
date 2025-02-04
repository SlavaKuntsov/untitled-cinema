using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class add_balance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("08a01170-5365-4c22-b52e-834d25b869e2"));

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "User",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Balance", "DateOfBirth", "Email", "FirstName", "LastName", "Password", "Role" },
                values: new object[] { new Guid("a592d29f-3c54-40f0-b7d7-40c0622a264e"), 0m, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@email.com", "admin", "admin", "$2a$11$UHwwtWYYZA9A4LqBR6xUTO5yDpkvxeG77q4//O4AWqyB7p1RcXgDG", "Admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a592d29f-3c54-40f0-b7d7-40c0622a264e"));

            migrationBuilder.DropColumn(
                name: "Balance",
                table: "User");

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "DateOfBirth", "Email", "FirstName", "LastName", "Password", "Role" },
                values: new object[] { new Guid("08a01170-5365-4c22-b52e-834d25b869e2"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@email.com", "", "", "$2a$11$HFMERE.0BFWNUVN6UGvrgOwe4lDPDgZ1g3kiyqkmcP17S32zq8ZS6", "Admin" });
        }
    }
}
