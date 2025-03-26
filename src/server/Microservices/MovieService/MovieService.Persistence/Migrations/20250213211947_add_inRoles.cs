using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class add_inRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InRoles",
                table: "Movie",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "Poster",
                table: "Movie",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.CreateTable(
                name: "MovieFrame",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Image = table.Column<byte[]>(type: "bytea", nullable: false),
                    MovieId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieFrame", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovieFrame_Movie_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovieFrame_MovieId",
                table: "MovieFrame",
                column: "MovieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovieFrame");

            migrationBuilder.DropColumn(
                name: "InRoles",
                table: "Movie");

            migrationBuilder.DropColumn(
                name: "Poster",
                table: "Movie");
        }
    }
}
