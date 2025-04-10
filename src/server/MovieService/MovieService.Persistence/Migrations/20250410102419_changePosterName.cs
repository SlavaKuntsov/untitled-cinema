using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class changePosterName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SeatType",
                keyColumn: "Id",
                keyValue: new Guid("53e58dc8-c87a-4b89-9ad0-9b1126fc699c"));

            migrationBuilder.RenameColumn(
                name: "PosterUrl",
                table: "Movie",
                newName: "Poster");

            migrationBuilder.InsertData(
                table: "SeatType",
                columns: new[] { "Id", "Name", "PriceModifier" },
                values: new object[] { new Guid("0d0581ed-7e0b-4272-b7f1-00d2b1625800"), "Стандарт", 1m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SeatType",
                keyColumn: "Id",
                keyValue: new Guid("0d0581ed-7e0b-4272-b7f1-00d2b1625800"));

            migrationBuilder.RenameColumn(
                name: "Poster",
                table: "Movie",
                newName: "PosterUrl");

            migrationBuilder.InsertData(
                table: "SeatType",
                columns: new[] { "Id", "Name", "PriceModifier" },
                values: new object[] { new Guid("53e58dc8-c87a-4b89-9ad0-9b1126fc699c"), "Стандарт", 1m });
        }
    }
}
