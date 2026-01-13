using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seats.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSeatRow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Row",
                table: "seats");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Row",
                table: "seats",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
