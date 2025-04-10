using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class changePosterType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Poster",
                table: "Movie");

            migrationBuilder.AddColumn<string>(
                name: "PosterUrl",
                table: "Movie",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "SeatType",
                columns: new[] { "Id", "Name", "PriceModifier" },
                values: new object[] { new Guid("53e58dc8-c87a-4b89-9ad0-9b1126fc699c"), "Стандарт", 1m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SeatType",
                keyColumn: "Id",
                keyValue: new Guid("53e58dc8-c87a-4b89-9ad0-9b1126fc699c"));

            migrationBuilder.DropColumn(
                name: "PosterUrl",
                table: "Movie");

            migrationBuilder.AddColumn<byte[]>(
                name: "Poster",
                table: "Movie",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
