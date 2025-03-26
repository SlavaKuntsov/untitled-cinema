using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class add_notifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a592d29f-3c54-40f0-b7d7-40c0622a264e"));

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notification_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Balance", "DateOfBirth", "Email", "FirstName", "LastName", "Password", "Role" },
                values: new object[] { new Guid("c1072bf0-aaae-4054-baf0-fa957fdce21e"), 0m, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@email.com", "admin", "admin", "$2a$11$YNtcUezN8AWzQQHBoqmRg.snL03rBk5pfVYnqHVdVcV0tjjzmfh7i", "Admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Notification_UserId",
                table: "Notification",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("c1072bf0-aaae-4054-baf0-fa957fdce21e"));

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Balance", "DateOfBirth", "Email", "FirstName", "LastName", "Password", "Role" },
                values: new object[] { new Guid("a592d29f-3c54-40f0-b7d7-40c0622a264e"), 0m, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@email.com", "admin", "admin", "$2a$11$UHwwtWYYZA9A4LqBR6xUTO5yDpkvxeG77q4//O4AWqyB7p1RcXgDG", "Admin" });
        }
    }
}
