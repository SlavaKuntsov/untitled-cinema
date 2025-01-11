using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class seats_array_int_json : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SeatsArrayJson",
                table: "Hall",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeatsArrayJson",
                table: "Hall");
        }
    }
}
