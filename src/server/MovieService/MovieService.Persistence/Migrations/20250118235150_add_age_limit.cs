using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class add_age_limit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "AgeLimit",
                table: "Movie",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgeLimit",
                table: "Movie");
        }
    }
}
