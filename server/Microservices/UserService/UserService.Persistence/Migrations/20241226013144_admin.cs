using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class admin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "DateOfBirth", "Email", "FirstName", "LastName", "Password", "Role" },
                values: new object[] { new Guid("94c7e850-8679-465c-9ece-46d0cc3f073d"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@gmail.com", "", "", "$2a$11$.5pNl0VTQRv1eAg0A2itiuQ0OgMXAn9xkiXZHMYdcmm83OfJ0rQi.", 3 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("94c7e850-8679-465c-9ece-46d0cc3f073d"));
        }
    }
}
