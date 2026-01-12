using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seats.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CompositeKeyUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_seats",
                table: "seats");

            migrationBuilder.AddPrimaryKey(
                name: "PK_seats",
                table: "seats",
                columns: new[] { "SeatId", "EventId", "FunctionId", "ZoneId", "VenueId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_seats",
                table: "seats");

            migrationBuilder.AddPrimaryKey(
                name: "PK_seats",
                table: "seats",
                column: "SeatId");
        }
    }
}
