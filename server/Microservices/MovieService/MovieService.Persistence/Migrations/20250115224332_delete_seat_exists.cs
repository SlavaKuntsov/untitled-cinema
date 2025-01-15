using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class delete_seat_exists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Exists",
                table: "Seat");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Exists",
                table: "Seat",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }
    }
}
