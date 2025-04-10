using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class movieFrame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SeatType",
                keyColumn: "Id",
                keyValue: new Guid("0d0581ed-7e0b-4272-b7f1-00d2b1625800"));

            migrationBuilder.DropColumn(
                name: "Image",
                table: "MovieFrame");

            migrationBuilder.AddColumn<string>(
                name: "FrameName",
                table: "MovieFrame",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "MovieFrame",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "SeatType",
                columns: new[] { "Id", "Name", "PriceModifier" },
                values: new object[] { new Guid("896a3142-1acf-4099-8ee5-2ab03e08610f"), "Стандарт", 1m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SeatType",
                keyColumn: "Id",
                keyValue: new Guid("896a3142-1acf-4099-8ee5-2ab03e08610f"));

            migrationBuilder.DropColumn(
                name: "FrameName",
                table: "MovieFrame");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "MovieFrame");

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "MovieFrame",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.InsertData(
                table: "SeatType",
                columns: new[] { "Id", "Name", "PriceModifier" },
                values: new object[] { new Guid("0d0581ed-7e0b-4272-b7f1-00d2b1625800"), "Стандарт", 1m });
        }
    }
}
