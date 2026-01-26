using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class change_birth_type : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("b93dd924-49fb-489f-bf50-46d1db20699f"));

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Balance", "DateOfBirth", "Email", "FirstName", "LastName", "Password", "Role" },
                values: new object[] { new Guid("cb7c0462-2eac-4233-a464-9623383715b5"), 0m, "01.01.0001", "admin@email.com", "admin", "admin", "$2a$11$7W0.dEc3LeGeNkNVUrB3eunfU1y8Vd/DgUuQtk4Fh59xRW6/c7kRW", "Admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("cb7c0462-2eac-4233-a464-9623383715b5"));

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Balance", "DateOfBirth", "Email", "FirstName", "LastName", "Password", "Role" },
                values: new object[] { new Guid("b93dd924-49fb-489f-bf50-46d1db20699f"), 0m, "01.01.0001 00:00:00", "admin@email.com", "admin", "admin", "$2a$11$.gGITqk8/BrzDztNvm2EvOQt11LCf6FbOmO2Ugoye9zZ3ch//r5la", "Admin" });
        }
    }
}
