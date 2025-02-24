using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class add_isDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("c1072bf0-aaae-4054-baf0-fa957fdce21e"));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Notification",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Balance", "DateOfBirth", "Email", "FirstName", "LastName", "Password", "Role" },
                values: new object[] { new Guid("060f6309-4dc5-4fc3-acac-d785abdcb89b"), 0m, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@email.com", "admin", "admin", "$2a$11$X81d3EHaabXN4VO.VAEfWOJu4trav244CYK7fbjNtlhxN2LecaTIa", "Admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("060f6309-4dc5-4fc3-acac-d785abdcb89b"));

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Notification");

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Balance", "DateOfBirth", "Email", "FirstName", "LastName", "Password", "Role" },
                values: new object[] { new Guid("c1072bf0-aaae-4054-baf0-fa957fdce21e"), 0m, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@email.com", "admin", "admin", "$2a$11$YNtcUezN8AWzQQHBoqmRg.snL03rBk5pfVYnqHVdVcV0tjjzmfh7i", "Admin" });
        }
    }
}
