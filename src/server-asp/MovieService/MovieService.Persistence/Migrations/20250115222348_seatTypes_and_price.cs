using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class seatTypes_and_price : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HallSeat_Hall_HallId",
                table: "HallSeat");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HallSeat",
                table: "HallSeat");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "HallSeat");

            migrationBuilder.DropColumn(
                name: "SeatType",
                table: "HallSeat");

            migrationBuilder.RenameTable(
                name: "HallSeat",
                newName: "Seat");

            migrationBuilder.RenameIndex(
                name: "IX_HallSeat_HallId",
                table: "Seat",
                newName: "IX_Seat_HallId");

            migrationBuilder.AddColumn<decimal>(
                name: "PriceModifier",
                table: "Session",
                type: "numeric(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Movie",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "SeatTypeId",
                table: "Seat",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Seat",
                table: "Seat",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "SeatType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PriceModifier = table.Column<decimal>(type: "numeric(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeatType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Seat_SeatTypeId",
                table: "Seat",
                column: "SeatTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Seat_Hall_HallId",
                table: "Seat",
                column: "HallId",
                principalTable: "Hall",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Seat_SeatType_SeatTypeId",
                table: "Seat",
                column: "SeatTypeId",
                principalTable: "SeatType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seat_Hall_HallId",
                table: "Seat");

            migrationBuilder.DropForeignKey(
                name: "FK_Seat_SeatType_SeatTypeId",
                table: "Seat");

            migrationBuilder.DropTable(
                name: "SeatType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Seat",
                table: "Seat");

            migrationBuilder.DropIndex(
                name: "IX_Seat_SeatTypeId",
                table: "Seat");

            migrationBuilder.DropColumn(
                name: "PriceModifier",
                table: "Session");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Movie");

            migrationBuilder.DropColumn(
                name: "SeatTypeId",
                table: "Seat");

            migrationBuilder.RenameTable(
                name: "Seat",
                newName: "HallSeat");

            migrationBuilder.RenameIndex(
                name: "IX_Seat_HallId",
                table: "HallSeat",
                newName: "IX_HallSeat_HallId");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "HallSeat",
                type: "numeric(10,2)",
                nullable: false,
                defaultValue: 0.00m);

            migrationBuilder.AddColumn<string>(
                name: "SeatType",
                table: "HallSeat",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HallSeat",
                table: "HallSeat",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HallSeat_Hall_HallId",
                table: "HallSeat",
                column: "HallId",
                principalTable: "Hall",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
